# ডিস্ট্রিবিউটেড সিস্টেমে সাগা প্যাটার্ন: ডিস্ট্রিবিউটেড ট্রানজ্যাকশন এবং ফেইলিউর রিকভারি (The Saga Pattern)

একটি একক রিলেশনাল ডাটাবেজযুক্ত মনোলিথিক অ্যাপ্লিকেশনে একাধিক টেবিল বা অপারেশনের মাঝে ডেটা কনসিস্টেন্সি রক্ষা করা খুবই সহজ: পুরো কাজটি একটি একক ডাটাবেজ ট্রানজ্যাকশনের মধ্যে সম্পাদন করে **ACID** (Atomicity, Consistency, Isolation, Durability) গ্যারান্টি কার্যকর করা যায়।

কিন্তু আধুনিক ক্লাউড-নেটিভ মাইক্রোসার্ভিস আর্কিটেকচারে প্রতিটি সার্ভিসের জন্য নিজস্ব প্রাইভেট ডাটাবেজ থাকে—যাকে বলা হয় **Database-per-Service** প্যাটার্ন। এর ফলে প্রতিটি মাইক্রোসার্ভিস তার নিজস্ব ডাটাবেজ (PostgreSQL, MongoDB, DynamoDB) স্বাধীনভাবে নিয়ন্ত্রণ করে। তবে বাস্তব জীবনের ব্যবসায়িক লেনদেন—যেমন একটি ই-কমার্স অর্ডার চেকআউট, হোটেল ও ফ্লাইট বুকিং, কিংবা রাইড-শেয়ারিং ট্রিপ—একাধিক স্বাধীন মাইক্রোসার্ভিসের মধ্য দিয়ে সম্পন্ন হয়।

**সাগা প্যাটার্ন (The Saga Pattern)** হলো এমন একটি ডিস্ট্রিবিউটেড আর্কিটেকচারাল প্যাটার্ন যা কোনো সেন্ট্রালাইজড ব্লকিং লক (Distributed Lock) ছাড়াই একগুচ্ছ **লোকাল ট্রানজ্যাকশন (Local Transactions)** এবং ব্যর্থতার ক্ষেত্রে **কম্পেনসেটিং ট্রানজ্যাকশন (Compensating Transactions)** ব্যবহারের মাধ্যমে স্বাধীন মাইক্রোসার্ভিসগুলোর মাঝে ইভেনচুয়াল কনসিস্টেন্সি (Eventual Data Consistency) নিশ্চিত করে।

![The Death of 2PC vs. The Saga Pattern](images/saga-problem-2pc-vs-saga.svg)

---

## ১. মূল সমস্যা: আমাদের সাগা প্যাটার্ন কেন প্রয়োজন? (Why Do We Need the Saga Pattern?)

### সনাতন ডিস্ট্রিবিউটেড ট্রানজ্যাকশনের ব্যর্থতা (The Death of 2PC / XA)
ক্লাউড প্রযুক্তির পূর্বে একাধিক ডাটাবেজের মধ্যে ট্রানজ্যাকশন সম্পন্ন করার জন্য **টু-ফেজ কমিট (Two-Phase Commit - 2PC)** প্রোটোকল (যেমন: X/Open XA স্ট্যান্ডার্ড) ব্যবহার করা হতো:
1. **ফেজ ১ (Prepare)**: একজন কেন্দ্রীয় সমন্বয়কারী (Transaction Coordinator) প্রতিটি ডাটাবেজকে জিজ্ঞাসা করে: *"তোমরা কি এই পরিবর্তনটি কমিট করতে প্রস্তুত?"* প্রতিটি ডাটাবেজ নির্দিষ্ট রোর ওপর লক (Row Lock) বসিয়ে উত্তর দেয় *"Prepared"*.
2. **ফেজ ২ (Commit)**: যদি সব অংশগ্রহণকারী হ্যাঁ ভোট দেয়, তবে সমন্বয়কারী সবাইকে কমিট করার নির্দেশ দেয়। যদি কোনো একটি সার্ভিস ব্যর্থ হয় বা টাইমআউট ঘটে, তবে সবাইকে রোলব্যাক করার নির্দেশ দেওয়া হয়।

### ক্লাউড মাইক্রোসার্ভিসে টু-ফেজ কমিট (2PC) কেন ব্যর্থ হয়?

| 2PC ব্যর্থতার কারণ | ক্লাউডে এটি কেন অচল? | বাস্তব প্রভাব |
| :--- | :--- | :--- |
| **ডিস্ট্রিবিউটেড রো লক (Row Locks)** | দূরবর্তী সার্ভিসগুলোর উত্তরের অপেক্ষায় প্রতিটি ডাটাবেজের রোর ওপর দীর্ঘক্ষণ লক ধরে রাখতে হয়। | ল্যাটেন্সি তীব্রভাবে বৃদ্ধি পায় ($T_{\text{hold}} = \sum T_{\text{network}}$); থ্রেড পুল শেষ হয়ে যায় এবং সিস্টেম থ্রুপুট ধসে পড়ে। |
| **কোঅর্ডিনেটর সিঙ্গেল পয়েন্ট অফ ফেইলিউর (SPOF)** | ফেজ ১ ও ফেজ ২-এর মধ্যবর্তী সময়ে সমন্বয়কারী সার্ভার ক্র্যাশ করলে ডাটাবেজগুলো **চিরতরে লক অবস্থায় আটকে থাকে**। | সিস্টেম ব্লক হয়ে যায়; মানুষের ম্যানুয়াল হস্তক্ষেপ ছাড়া ডাটাবেজ আনলক করা যায় না। |
| **ক্লাউড ও NoSQL-এ কোনো সাপোর্ট নেই** | আধুনিক ক্লাউড ডাটাবেজ যেমন **Amazon DynamoDB, Apache Kafka, MongoDB, এবং Stripe API**-তে XA/2PC সমর্থন নেই। | একটি লোকাল SQL ডাটাবেজ এবং থার্ড-পার্টি পেমেন্ট গেটওয়ের মাঝে কখনোই 2PC ট্রানজ্যাকশন চালানো সম্ভব নয়! |
| **CAP উপপাদ্যের CP ফাঁদ** | 2PC প্রাপ্যতা (Availability) ত্যাগ করে কনসিস্টেন্সিকে (Consistency) প্রাধান্য দেয় ($CP$)। নেটওয়ার্ক বিভ্রাট ঘটলে পুরো সিস্টেম ডেডলকে পড়ে যায়। | আধুনিক ওয়েব অ্যাপ্লিকেশনে এটি অগ্রহণযোগ্য, যেখানে উচ্চ প্রাপ্যতা ($AP$) বাধ্যতামূলক। |

### দ্য ডুয়াল-রাইট প্রবলেম (The Dual-Write Problem)
একই সাথে লোকাল ডাটাবেজে ডেটা সেভ করা এবং মেসেজ ব্রোকারে ইভেন্ট পাবলিশ করার চেষ্টাকে বলা হয় **ডুয়াল-রাইট সমস্যা**:
- ডাটাবেজে সেভ সফল হলো কিন্তু ব্রোকারে মেসেজ যাওয়ার আগেই সার্ভার ক্র্যাশ করলে পরবর্তী সার্ভিসগুলো কিছুই জানবে না।
- আগে মেসেজ পাঠিয়ে দিলে কিন্তু পরে লোকাল ডাটাবেজ ট্রানজ্যাকশন ফেইল করলে সিস্টেমে অসত্য ডেটা তৈরি হবে!

**সাগা প্যাটার্ন—ট্রানজ্যাকশনাল আউটবক্স প্যাটার্নের (Transactional Outbox Pattern) সাথে মিলে এই সমস্যার সম্পূর্ণ সমাধান করে।**

---

## ২. সাগা কী? মূল কার্যপ্রণালী এবং ভিত্তি (Saga Fundamentals)

সাগা ধারণাটি ১৯৮৭ সালে প্রিন্সটন বিশ্ববিদ্যালয়ের গবেষক **হেক্টর গার্সিয়া-মোলিনা এবং কেনেথ সালেম (Hector Garcia-Molina & Kenneth Salem)** তাদের বিখ্যাত গবেষণাপত্র *"Sagas"*-এ প্রথম উপস্থাপন করেন। মূলত দীর্ঘমেয়াদী ডাটাবেজ ট্রানজ্যাকশন সামলাতে এটি তৈরি হলেও বর্তমানে এটি মাইক্রোসার্ভিসের অপরিহার্য প্যাটার্ন।

### সাগার সংজ্ঞা
একটি **সাগা** হলো একাধিক স্বাধীন লোকাল ট্রানজ্যাকশনের একটি ধারাবাহিক পর্যায়ক্রম:
$$S = \{T_1, \, T_2, \, T_3, \, \dots, \, T_n\}$$

- প্রতিটি লোকাল ট্রানজ্যাকশন $T_i$ একটি নির্দিষ্ট মাইক্রোসার্ভিসের নিজস্ব ডাটাবেজের মধ্যে স্থানীয় ACID ট্রানজ্যাকশন সম্পন্ন করে।
- স্থানীয়ভাবে ডেটা কমিট হওয়ার পর সার্ভিসটি একটি মেসেজ বা ইভেন্ট পাবলিশ করে যা পরবর্তী সার্ভিসটির লোকাল ট্রানজ্যাকশন $T_{i+1}$ শুরু করে।

---

### ব্যর্থতা সামলানো: ফরোয়ার্ড রিকভারি বনাম ব্যাকওয়ার্ড রিকভারি

সাগার মাঝপথে কোনো একটি লোকাল ট্রানজ্যাকশন ব্যর্থ হলে (যেমন: $T_3$ এরর দিল), পূর্বে সফল হওয়া $T_1$ এবং $T_2$-কে সরাসরি ডাটাবেজ `ROLLBACK` করা সম্ভব নয়, কারণ তারা **ইতোমধ্যেই ডাটাবেজ ডিস্কে স্থায়ীভাবে সেভ হয়ে গেছে**। সাগাতে দুটি রিকভারি কৌশল ব্যবহৃত হয়:

```
                            সাগা ফেইলিউর রিকভারি কৌশল
                                        │
                ┌───────────────────────┴───────────────────────┐
                ▼                                               ▼
       ফরোয়ার্ড রিকভারি                                ব্যাকওয়ার্ড রিকভারি
    (Forward Recovery / Retry)                     (Compensating Transactions)
 • সাফল্য না আসা পর্যন্ত রিট্রাই                • ব্যবসায়িক ব্যর্থতা ঘটলে ব্যবহৃত হয়
 • যেসব অপারেশন ব্যর্থ হতে পারে না                 (যেমন: কার্ড ডিক্লাইন, স্টক শেষ)
 • উদাহরণ: নিশ্চিতকরণ ইমেইল পাঠানো             • রিভার্স সেমান্টিক আনডু চালায়: C2, C1
```

#### ক. ফরোয়ার্ড রিকভারি (Forward Recovery - সাফল্য না আসা পর্যন্ত রিট্রাই)
যেসব অপারেশন ব্যবসায়িক কারণে কখনোই বাতিল হতে পারে না এবং কেবল সাময়িক নেটওয়ার্ক সমস্যার কারণে আটকে আছে (যেমন: কনফার্মেশন ইমেইল পাঠানো, পিডিএফ ইনভয়েস তৈরি), সেগুলোকে এক্সপোনেনশিয়াল ব্যাকঅফ এবং আইডেমপোটেন্ট রিট্রাইয়ের মাধ্যমে সফল না হওয়া পর্যন্ত পুনরায় চালানো হয়।

#### খ. ব্যাকওয়ার্ড রিকভারি (Backward Recovery - কম্পেনসেটিং ট্রানজ্যাকশন)
যখন কোনো অপরিবর্তনীয় ব্যবসায়িক ত্রুটি ঘটে (যেমন: অ্যাকাউন্টে টাকা নেই, কার্ড ডিক্লাইন, জালিয়াতি ধরা পড়া), তখন সাগা পূর্বে সফল হওয়া প্রতিটি ধাপের জন্য বিপরীতমুখী **কম্পেনসেটিং ট্রানজ্যাকশন ($C_i$)** উল্টো ক্রমানুসারে চালায়:

$$\text{রোলব্যাক সিকোয়েন্স:} \quad C_{n-1}, \, C_{n-2}, \, \dots, \, C_1$$

---

### অত্যন্ত গুরুত্বপূর্ণ ধারণা: সেমান্টিক আনডু বনাম স্টোরেজ রোলব্যাক
একক SQL ডাটাবেজে `ROLLBACK` কমান্ড দিলে ট্রানজ্যাকশন লগ এমনভাবে মুছে যায় যেন কোনো পরিবর্তন কখনোই ঘটেনি।

কিন্তু সাগাতে **ঘণ্টার শব্দকে ফিরিয়ে নেওয়া যায় না (You cannot un-ring a bell)**। একটি কম্পেনসেটিং ট্রানজ্যাকশন হলো একটি **নতুন ব্যবসায়িক প্রতিপূরক অ্যাকশন (Offsetting Action)**:
- চার্জ করা ক্রেডিট কার্ড আনডু করা যায় না; আপনাকে একটি নতুন **রিফান্ড (Refund)** ট্রানজ্যাকশন করতে হয়।
- ডাটাবেজে সেভ হওয়া অর্ডার ডিলিট করা যায় না; তার স্ট্যাটাস পরিবর্তন করে **`CANCELLED`** বা **`REJECTED`** করতে হয়।
- বুক করা হোটেলের ঘর মুছে ফেলা যায় না; একটি **বাতিলকরণ নোটিশ** পাঠাতে হয়।

ডিস্ট্রিবিউটেড সিস্টেমে ইতিহাস কখনো মোছা যায় না, কেবল **নতুন তথ্যের সংযোজন (Append-Only)** ঘটে।

---

## ৩. কোরিওগ্রাফি বনাম অর্কেস্ট্রেশন: আর্কিটেকচারাল তুলনা (Choreography vs. Orchestration)

সাগা বাস্তবায়নে দুটি প্রধান আর্কিটেকচারাল পদ্ধতি বিদ্যমান: **কোরিওগ্রাফি (Choreography - বিকেন্দ্রীভূত)** এবং **অর্কেস্ট্রেশন (Orchestration - কেন্দ্রীভূত)**।

![Saga Execution Models: Choreography vs. Orchestration](images/choreography-vs-orchestration.svg)

### ক. কোরিওগ্রাফি-ভিত্তিক সাগা (Choreography - Decentralized Pub/Sub)
কোরিওগ্রাফিতে কোনো কেন্দ্রীয় নিয়ন্ত্রক থাকে না। মাইক্রোসার্ভিসগুলো পূর্ববর্তী সার্ভিসের ডোমেন ইভেন্ট শুনে স্বাধীনভাবে নিজেদের লোকাল কাজ সম্পন্ন করে নতুন ইভেন্ট পাবলিশ করে:

```
[Order Svc] ──OrderCreated──► [Inventory Svc] ──InventoryReserved──► [Payment Svc]
```

#### সুবিধাসমূহ:
- **লুজ কাপলিং (Loose Coupling)**: কোনো কেন্দ্রীয় কর্তৃপক্ষ সার্ভিসগুলোকে নিয়ন্ত্রণ করে না; সবাই ইভেন্ট আদান-প্রদান করে।
- **ছোট সিস্টেমের জন্য সহজ**: ২ থেকে ৩টি সার্ভিসের ছোট ওয়ার্কফ্লোতে খুব দ্রুত বাস্তবায়ন করা যায়।

#### দুর্বলতা ও অসুবিধা:
- **"পিনবল মেশিন" আর্কিটেকচার (Pinball Machine)**: ইভেন্টগুলো সার্ভিসের মাঝে বলের মতো ধাক্কা খেতে থাকে। পুরো ফ্লো বোঝার জন্য একাধিক রিপোজিটরি ও কোডবেস পরীক্ষা করতে হয়।
- **সাইক্লিক ডিপেন্ডেন্সি**: প্রায়শই সার্ভিসগুলোকে একে অপরের ইভেন্ট সাবস্ক্রাইব করতে হয়, যা অদৃশ্য টাইট কাপলিং তৈরি করে।
- **রোলব্যাকের চরম জটিলতা**: কোনো ধাপে এরর হলে ৪-৫টি সার্ভিসের মধ্য দিয়ে রিভার্স ইভেন্ট পাঠিয়ে রোলব্যাক পরিচালনা করা অত্যন্ত কষ্টকর ও বাগ-প্রবণ।

---

### খ. অর্কেস্ট্রেশন-ভিত্তিক সাগা (Orchestration - Centralized State Machine)
অর্কেস্ট্রেশনে একটি ডেডিকেটেড **সাগা অর্কেস্ট্রেটর (Saga Orchestrator)** থাকে যা একটি স্টেট মেশিন (State Machine) হিসেবে কাজ করে। এটি অংশগ্রহণকারী সার্ভিসগুলোকে সুনির্দিষ্ট **কমান্ড (Commands)** পাঠায় এবং ফলাফল বা ইভেন্ট গ্রহণ করে:

```
                  ┌───────────────┐
                  │ Order Saga    │
                  │ Orchestrator  │
                  └───┬───────┬───┘
       ReserveInventory│       │ProcessPayment
           Command    │       │   Command
                      ▼       ▼
              [Inventory]   [Payment]
```

#### সুবিধাসমূহ:
- **একক সত্যের উৎস (Single Source of Truth)**: অর্কেস্ট্রেটরের ডাটাবেজ টেবিলেই প্রতিটি লেনদেনের বর্তমান অবস্থা (যেমন: `AwaitingPayment`, `Compensating`, `Completed`) স্পষ্ট সংরক্ষিত থাকে।
- **দায়িত্বের পৃথকীকরণ (Separation of Concerns)**: মাইক্রোসার্ভিসগুলো শুধুমাত্র কর্মী হিসেবে কমান্ড কার্যকর করে; তাদেরকে পুরো সিস্টেমের জটিল ব্যবসায়িক ফ্লো মনে রাখতে হয় না।
- **সহজ ও নির্ভুল রোলব্যাক**: কোনো ধাপ ব্যর্থ হলে অর্কেস্ট্রেটর নিজেই পূর্ববর্তী সার্ভিসগুলোকে সুনির্দিষ্ট কম্পেনসেটিং কমান্ড পাঠিয়ে দেয়।
- **অসাধারণ পর্যবেক্ষণযোগ্যতা (Observability)**: দীর্ঘমেয়াদী ব্যবসায়িক ফ্লো (যা সম্পন্ন হতে কয়েক মিনিট বা দিন লাগতে পারে) একটিমাত্র ড্যাশবোর্ড থেকেই পর্যবেক্ষণ করা যায়।

#### দুর্বলতা:
- **অতিরিক্ত কেন্দ্রীকরণের ঝুঁকি**: ডেভেলপাররা যদি সার্ভিসগুলোর নিজস্ব লজিক অর্কেস্ট্রেটরের ভেতর লিখে ফেলে, তবে অর্কেস্ট্রেটর একটি ভঙ্গুর ও জটিল "গড অবজেক্ট (God Object)"-এ পরিণত হতে পারে।

---

### পাশাপাশি আর্কিটেকচারাল ট্রেড-অফ ম্যাট্রিক্স

| আর্কিটেকচারাল মাত্রা | কোরিওগ্রাফি-ভিত্তিক সাগা (Choreography) | অর্কেস্ট্রেশন-ভিত্তিক সাগা (Orchestration) |
| :--- | :--- | :--- |
| **সমন্বয়ের ধরন** | বিকেন্দ্রীভূত পিয়ার-টু-পিয়ার ইভেন্ট মেশ। | কেন্দ্রীভূত হাব-অ্যান্ড-স্পোক স্টেট মেশিন। |
| **কমিউনিকেশন স্টাইল** | অ্যাসিঙ্ক্রোনাস ডোমেন ইভেন্ট (`OrderCreated`). | সুনির্দিষ্ট পয়েন্ট-টু-পয়েন্ট কমান্ড (`ProcessPayment`). |
| **কাপলিং** | কাঠামোগত কাপলিং কম হলেও ইভেন্ট কাপলিং অত্যন্ত বেশি। | অংশগ্রহণকারী সার্ভিসের কাপলিং অত্যন্ত কম। |
| **স্টেট পর্যবেক্ষণযোগ্যতা** | প্রতিটি মাইক্রোসার্ভিসের ডাটাবেজে ছড়িয়ে থাকে। | **সম্পূর্ণ কেন্দ্রীভূত**: অর্কেস্ট্রেটর টেবিলে সংরক্ষিত। |
| **রোলব্যাক জটিলতা** | **অত্যন্ত জটিল**: রিভার্স ইভেন্ট সাবস্ক্রিপশন কঠিন। | **সহজ ও সরল**: অর্কেস্ট্রেটর নিজেই রোলব্যাক চালায়। |
| **সেরা ব্যবহারের ক্ষেত্র** | সাধারণ ২ – ৩ ধাপের সরল ওয়ার্কফ্লো। | এন্টারপ্রাইজ জটিল ব্যবসায়িক সিস্টেম (ই-কমার্স, ব্যাংকিং)। |

---

## ৪. সাগাতে আইসোলেশনের অভাব (ACD without I) এবং কনকারেন্সি সমাধান

সাগা আর্কিটেকচারের সবচেয়ে ঝুঁকিপূর্ণ দিক হলো কনকারেন্সি (Concurrency)। সাগা **Atomicity**, **Consistency**, এবং **Durability** নিশ্চিত করে, কিন্তু এতে **ISOLATION থাকে না**!

$$\text{সাগা নিশ্চিত করে } \mathbf{ACD} \text{, কখনোই } \mathbf{ACID} \text{ নয়}$$

যেহেতু প্রতিটি লোকাল ট্রানজ্যাকশন ডাটাবেজে তাৎক্ষণিকভাবে কমিট হয়ে যায়, তাই **অন্যান্য সমসাময়িক ট্রানজ্যাকশনগুলো সাগা সম্পন্ন হওয়ার আগেই মাঝপথের অসম্পূর্ণ ডেটা দেখতে পায়**।

### প্রধান কনকারেন্সি সমস্যাসমূহ
1. **লস্ট আপডেটস (Lost Updates)**: সাগা ১ একটি ব্যালেন্স আপডেট করল; সমসাময়িক সাগা ২ সাগা ১ সম্পন্ন হওয়ার আগেই সেই ব্যালেন্স ওভাররাইট করে ফেলল।
2. **ডার্টি রিডস (Dirty Reads)**: সাগা ১ একটি অর্ডার তৈরি করে বিমানের সিট বুক করল। একজন ব্যবহারকারী সার্চ করে দেখল কোনো সিট খালি নেই। একটু পর সাগা ১ পেমেন্ট ফেইল করায় সিট বুকিং বাতিল (Compensate) করল। ব্যবহারকারী এখানে ডার্টি রিড দেখতে পেয়েছিল।

---

### প্রমাণিত আর্কিটেকচারাল সমাধানসমূহ (Countermeasures)

আইসোলেশন ছাড়া নিরাপদে সাগা চালানোর জন্য ইন্ডাস্ট্রিতে নিচের কৌশলগুলো প্রয়োগ করা হয়:

```
                            সাগা আইসোলেশন সমাধান কৌশল
                                        │
    ┌───────────────────────┬───────────┴───────────┬───────────────────────┐
    ▼                       ▼                       ▼                       ▼
১. সেমান্টিক লক        ২. কমিউটেটিভ আপডেট     ৩. পেসিমিস্টিক ভিউ      ৪. অপটিমিস্টিক লক
 • Status = PENDING     • ক্রম পরিবর্তন হলেও    • অপরিবর্তনীয় ধাপ      • RowVersion / xmin
 • অ্যাপ লেভেলে নতুন      ফলাফল একই থাকে          (Payment) সবার          • সমসাময়িক সংঘর্ষ
   রাইট ব্লক করে          (Credit / Debit)        শেষে রাখা হয়           শনাক্ত ও রিট্রাই করে
```

#### ১. সেমান্টিক লক (Semantic Lock / Pending State)
যখনই কোনো লোকাল ট্রানজ্যাকশন ডেটা পরিবর্তন করে, তখন সে একটি ফ্ল্যাগ সেট করে দেয় (যেমন: `OrderState = PENDING_PAYMENT`, `Stock = RESERVED`)। অন্যান্য সমসাময়িক সাগা এই ফ্ল্যাগ দেখে বুঝতে পারে যে রেকর্ডটি বর্তমানে একটি চলমান লেনদেনের অধীন এবং সেটিকে পরিবর্তন করা থেকে বিরত থাকে।

#### ২. কমিউটেটিভ আপডেট (Commutative Updates)
অপারেশনগুলোকে এমনভাবে ডিজাইন করা যাতে তাদের কার্যকর করার ক্রম পরিবর্তনের ফলেও চূড়ান্ত ফলাফল একই থাকে:
$$\text{Account.Balance} + 50 - 30 = \text{Account.Balance} - 30 + 50$$
এটি লস্ট আপডেট সমস্যা প্রতিরোধ করে।

#### ৩. পেসিমিস্টিক ভিউ / ধাপের পুনর্বিন্যাস (Pessimistic View)
ব্যবসায়িক ঝুঁকি কমাতে সাগার ধাপগুলোকে সাজান:
- সহজে বাতিলযোগ্য ধাপগুলো (ইনভেন্টরি সাময়িক ব্লক করা, পয়েন্ট হোল্ড করা) **প্রথমে** চালান।
- কঠিন বা অপরিবর্তনীয় ধাপগুলো (ক্রেডিট কার্ড থেকে নগদ টাকা কাটা, কারখানায় জিনিস তৈরি শুরু করা) সবার **শেষে** চালান।

#### ৪. অপটিমিস্টিক কনকারেন্সি কন্ট্রোল (OCC)
সাগা স্টেট টেবিলে একটি `RowVersion` বা `xmin` টোকেন রাখুন। যদি দুটি মেসেজ একই সাথে স্টেট পরিবর্তন করতে আসে, তবে দ্বিতীয়টি অপটিমিস্টিক কনকারেন্সি এক্সেপশন খেয়ে নিরাপদভাবে রিট্রাই করবে।

---

## ৫. বাস্তব ই-কমার্স সাগা ফেইলিউর সিকোয়েন্স (Real-World Case Study)

একটি আধুনিক ই-কমার্স সিস্টেমের ফেইলিউর ও কম্পেনসেশন ফ্লো লক্ষ্য করুন:

![Saga Failure & Semantic Compensation Flow](images/compensating-transaction-flow.svg)

### স্বাভাবিক সফল পথ (Happy Path Execution)
1. **Order Service**: `PENDING` অবস্থায় অর্ডার তৈরি করে।
2. **Inventory Service**: স্টক থেকে ২টি আইটেম বিয়োগ করে।
3. **Payment Service**: ক্রেডিট কার্ড থেকে $১৫০ চার্জ করে।
4. **Order Service**: অর্ডারের অবস্থা `COMPLETED` করে এবং কনফার্মেশন পাঠানো হয়।

---

### ব্যর্থতা ও কম্পেনসেশন পথ (Backward Recovery)
1. **Order Service**: `PENDING` অবস্থায় অর্ডার তৈরি করে।
2. **Inventory Service**: স্টক থেকে ২টি আইটেম বিয়োগ করে।
3. **Payment Service**: কার্ডটি **ডিক্লাইনড (Declined)** হলো (অপর্যাপ্ত ব্যালেন্স বা জালিয়াতি)।
4. **Saga Orchestrator**: `PaymentFailed` ইভেন্ট গ্রহণ করে।
5. **কম্পেনসেশন ধাপ ১**: অর্কেস্ট্রেটর ইনভেন্টরি সার্ভিসকে `ReleaseInventoryCommand` পাঠায়। ইনভেন্টরি স্টক আবার ২ বাড়িয়ে দেয়।
6. **কম্পেনসেশন ধাপ ২**: অর্কেস্ট্রেটর অর্ডার সার্ভিসকে `CancelOrderCommand` পাঠায়। অর্ডারের অবস্থা `REJECTED` হয়ে যায়।
7. **গ্রাহককে নোটিফিকেশন**: গ্রাহককে মেসেজ পাঠানো হয়: *"পেমেন্ট ব্যর্থ হয়েছে। আপনার অর্ডার বাতিল করা হয়েছে এবং পণ্যটি স্টকে ফেরত পাঠানো হয়েছে।"*

তিনটি স্বাধীন ডাটাবেজের মাঝেই নিখুঁত ডেটা কনসিস্টেন্সি নিশ্চিত হলো!

---

## ৬. আর্কিটেকচারাল স্ট্যান্ডার্ড (.NET / C# ফোকাস সহযোগে MassTransit)

.NET ইকোসিস্টেমে প্রোডাকশন-গ্রেড ডিস্ট্রিবিউটেড সাগা তৈরির জন্য **MassTransit Automatonymous State Machine** হলো সর্বাধিক ব্যবহৃত গোল্ড-স্ট্যান্ডার্ড ফ্রেমওয়ার্ক।

![MassTransit State Machine Saga Architecture](images/masstransit-state-machine-saga.svg)

### সুবিধাসমূহ:
- **ফ্লুয়েন্ট C# DSL**: স্টেট, ইভেন্ট এবং ট্রানজিশন পরিষ্কার C# সিনট্যাক্সে সংজ্ঞায়িত করা যায়।
- **বিল্ট-ইন ট্রানজ্যাকশনাল আউটবক্স**: সাগা স্টেট আপডেট এবং মেসেজ পাবলিশ একই লোকাল ACID ট্রানজ্যাকশনে সম্পন্ন হয়।
- **অপটিমিস্টিক কনকারেন্সি**: EF Core-এর সাহায্যে রেস কন্ডিশন স্বয়ংক্রিয়ভাবে প্রতিরোধ করে।

### দুর্বলতা ও ট্রেড-অফ:
- **স্কিমা মাইগ্রেশন**: চলমান (in-flight) সাগা চলাকালীন ডাটাবেজ স্কিমা পরিবর্তনের ক্ষেত্রে সাবধানতা অবলম্বন করতে হয়।

### লক্ষ্যযুক্ত প্রযুক্তি ও .NET-এর জন্য NuGet প্যাকেজ:
- `MassTransit` (মূল মেসেজিং ফ্রেমওয়ার্ক)
- `MassTransit.RabbitMQ` (অথবা `MassTransit.Azure.ServiceBus.Core`)
- `MassTransit.EntityFrameworkCoreIntegration` (সাগা স্টেট পারসিস্টেন্স)
- `Microsoft.EntityFrameworkCore` ও `Npgsql.EntityFrameworkCore.PostgreSQL`

---

### প্রোডাকশন-গ্রেড C# বাস্তবায়ন: ই-কমার্স MassTransit সাগা

#### ১. সাগা স্টেট এবং মেসেজ চুক্তি (Contracts)
```csharp
// Contracts.cs - Guid দ্বারা কোরিলেটেড অ্যাসিঙ্ক্রোনাস মেসেজসমূহ
namespace ECommerce.Contracts;

public record SubmitOrder(Guid CorrelationId, string OrderId, decimal Amount, int Quantity);
public record ReserveInventory(Guid CorrelationId, int Quantity);
public record InventoryReserved(Guid CorrelationId);
public record ProcessPayment(Guid CorrelationId, decimal Amount);
public record PaymentCompleted(Guid CorrelationId);
public record PaymentFailed(Guid CorrelationId, string Reason);
public record ReleaseInventory(Guid CorrelationId, int Quantity);
public record CancelOrder(Guid CorrelationId, string Reason);

// OrderState.cs - ডাটাবেজে সংরক্ষিত সাগা স্টেট সত্ত্বা
using MassTransit;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; } // প্রাইমারি কি (সাগা ইন্সট্যান্স আইডি)
    public string CurrentState { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public uint RowVersion { get; set; } // অপটিমিস্টিক কনকারেন্সি টোকেন (PostgreSQL xmin)
}
```

---

#### ২. MassTransit স্টেট মেশিন অর্কেস্ট্রেটর (OrderStateMachine)
```csharp
// OrderStateMachine.cs - সেন্ট্রাল অর্কেস্ট্রেটর
using ECommerce.Contracts;
using MassTransit;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    // স্টেটসমূহ
    public State Submitted { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;
    public State Faulted { get; private set; } = null!;

    // ইভেন্টসমূহ
    public Event<SubmitOrder> OrderSubmitted { get; private set; } = null!;
    public Event<InventoryReserved> InventoryReserved { get; private set; } = null!;
    public Event<PaymentCompleted> PaymentCompleted { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        // CorrelationId দিয়ে ইভেন্ট কোরিলেট করা হচ্ছে
        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => InventoryReserved, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentCompleted, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentFailed, x => x.CorrelateById(m => m.Message.CorrelationId));

        // ধাপ ১: ইনিশিয়াল অর্ডার সাবমিশন গ্রহণ
        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.Amount = context.Message.Amount;
                })
                .Publish(context => new ReserveInventory(context.Saga.CorrelationId, context.Saga.Quantity))
                .TransitionTo(Submitted)
        );

        // ধাপ ২: ইনভেন্টরি সফল হলে পেমেন্ট শুরু
        During(Submitted,
            When(InventoryReserved)
                .Publish(context => new ProcessPayment(context.Saga.CorrelationId, context.Saga.Amount))
                .TransitionTo(AwaitingPayment)
        );

        // ধাপ ৩: পেমেন্টের ফলাফল অনুযায়ী পরবর্তী পদক্ষেপ
        During(AwaitingPayment,
            // সফল পথ: পেমেন্ট সফল হলে সাগা সমাপ্ত
            When(PaymentCompleted)
                .Finalize(),

            // ব্যর্থ পথ: পেমেন্ট ফেইল করলে কম্পেনসেটিং ট্রানজ্যাকশন কার্যকর!
            When(PaymentFailed)
                .Publish(context => new ReleaseInventory(context.Saga.CorrelationId, context.Saga.Quantity))
                .Publish(context => new CancelOrder(context.Saga.CorrelationId, context.Message.Reason))
                .TransitionTo(Faulted)
        );

        // কাজ শেষে সম্পন্ন সাগা ক্লিনআপ করা হবে
        SetCompletedWhenFinalized();
    }
}
```

---

#### ৩. Program.cs - EF Core সাগা রিপোজিটরি এবং আউটবক্স কনফিগারেশন
```csharp
// Program.cs - PostgreSQL এবং ট্রানজ্যাকশনাল আউটবক্স সহ MassTransit
using ECommerce.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SagaDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Database=saga_db;Username=postgres;Password=secret");
});

builder.Services.AddMassTransit(x =>
{
    // স্টেট মেশিন এবং EF Core সাগা রিপোজিটরি রেজিস্ট্রেশন
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<SagaDbContext>();
            r.UsePostgres();
        });

    // RabbitMQ ট্রান্সপোর্ট এবং ট্রানজ্যাকশনাল আউটবক্স
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        // ডুয়াল-রাইট বাগ প্রতিরোধে বাধ্যতামূলক আউটবক্স প্যাটার্ন
        cfg.UseEntityFrameworkOutbox<SagaDbContext>(context);

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// নতুন সাগা শুরু করার এন্ডপয়েন্ট
app.MapPost("/api/checkout", async (IPublishEndpoint publishEndpoint) =>
{
    var sagaId = Guid.NewGuid();
    await publishEndpoint.Publish(new SubmitOrder(sagaId, $"ORD-{Random.Shared.Next(1000, 9999)}", 149.99m, 2));

    return Results.Accepted($"/api/orders/{sagaId}", new { CorrelationId = sagaId, Status = "Order Processing Started" });
});

app.Run();

// EF Core DbContext ডেফিনিশন
public class SagaDbContext(DbContextOptions<SagaDbContext> options) : DbContext(options)
{
    public DbSet<OrderState> OrderStates => Set<OrderState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OrderState>(entity =>
        {
            entity.HasKey(e => e.CorrelationId);
            entity.Property(e => e.CurrentState).HasMaxLength(64);
            entity.Property(e => e.RowVersion).IsRowVersion(); // অপটিমিস্টিক কনকারেন্সি
        });
    }
}
```

---

## ৭. সিস্টেম ডিজাইন ইন্টারভিউ ডিসিশন ফ্রেমওয়ার্ক (Saga Decision Tree)

ইন্টারভিউতে ডিস্ট্রিবিউটেড লেনদেন ও ডেটা কনসিস্টেন্সি সংক্রান্ত প্রশ্নের উত্তরে এই কাঠামোটি ব্যবহার করুন:

```
ডিস্ট্রিবিউটেড ডেটা কনসিস্টেন্সি সিদ্ধান্ত কাঠামো
 ├── সমস্ত অপারেশন কি একটি একক রিলেশনাল ডাটাবেজে রাখা সম্ভব?
 │    └── হ্যাঁ ──► Monolith / Modular Monolith ACID Transaction (কখনই অসময়ে সাগা ব্যবহার করবেন না!)
 │
 ├── ভিন্ন ডাটাবেজের মাঝে কি অ্যাসিঙ্ক্রোনাস ইভেন্ট প্রয়োজন?
 │    │
 │    ├── কেবল ২ থেকে ৩টি সার্ভিস এবং অত্যন্ত সাধারণ লজিক?
 │    │    └── হ্যাঁ ──► কোরিওগ্রাফি সাগা (Choreography - Kafka/RabbitMQ)
 │    │
 │    └── ৪টির বেশি সার্ভিস, জটিল ব্রাঞ্চিং কিংবা অডিটের প্রয়োজন আছে?
 │         └── হ্যাঁ ──► অর্কেস্ট্রেশন সাগা (Orchestration - MassTransit / Temporal State Machine)
 │
 ├── ডাটাবেজ ও মেসেজ ব্রোকারের মাঝে ডুয়াল-রাইট বাগ কীভাবে রোধ করবেন?
 │    └── বাধ্যতামূলক: ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন (Transactional Outbox Pattern)
 │
 └── ডাটাবেজ আইসোলেশনের অভাব কীভাবে সামলাবেন?
      ├── ধাপ ১: সেমান্টিক লক ব্যবহার করুন (Status = PENDING)
      ├── ধাপ ২: সম্ভব হলে কমিউটেটিভ আপডেট ডিজাইন করুন
      └── ধাপ ৩: অপরিবর্তনীয় ধাপগুলো (পেমেন্ট চার্জ) সবার শেষে রাখুন (Pessimistic View)
```

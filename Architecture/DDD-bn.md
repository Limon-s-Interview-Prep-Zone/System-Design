# ডোমেন-ড্রিভেন ডিজাইন (Domain-Driven Design - DDD): মূলনীতি, আর্কিটেকচার এবং স্টাফ-লেভেল ইন্টারভিউ সহায়িকা

ডোমেন-ড্রিভেন ডিজাইন (Domain-Driven Design বা DDD), ২০০৩ সালে এরিক ইভান্স (Eric Evans) কর্তৃক প্রবর্তিত এবং পরবর্তীতে ভন ভার্নন (Vaughn Vernon) ও মার্টিন ফাউলার (Martin Fowler) কর্তৃক জনপ্রিয়কৃত একটি আধুনিক সফটওয়্যার আর্কিটেকচার দর্শন। এই পদ্ধতিতে সফটওয়্যারের ডিজাইন সম্পূর্ণভাবে মূল বিজনেস ডোমেনের গভীর ও পরিবর্তনশীল মডেলের উপর কেন্দ্র করে গড়ে তোলা হয়।

আধুনিক এন্টারপ্রাইজ আর্কিটেকচারে—বিশেষ করে ডিস্ট্রিবিউটেড সিস্টেমস (Distributed Systems), মডুলার মনোলিথ (Modular Monolith), এবং মাইক্রোসার্ভিসেস (Microservices)-এ—জটিল বিজনেস লজিক যেন অনিয়ন্ত্রিত বিশৃঙ্খলায় (Big Ball of Mud) রূপ না নেয়, সেজন্য DDD কৌশলগত স্পষ্টতা (Strategic Design) এবং ব্যবহারিক গাঠনিক উপাদান (Tactical Design) সরবরাহ করে।

---

## ১. মূল দর্শন: প্রবলেম স্পেস বনাম সলিউশন স্পেস (Problem Space vs. Solution Space)

ডোমেন-ড্রিভেন ডিজাইন সফটওয়্যার ইঞ্জিনিয়ারিং প্রক্রিয়াকে দুটি মৌলিক ক্ষেত্রে বিভক্ত করে:

| মাত্রা (Dimension) | পরিধি (Scope) | মূল নিদর্শন (Key Artifacts) | প্রাথমিক লক্ষ্য (Primary Goal) |
| :--- | :--- | :--- | :--- |
| **প্রবলেম স্পেস (Problem Space)** | বিজনেস ডোমেন বিশ্লেষণ (Business Analysis) | সাবডোমেন (Core, Supporting, Generic), বিজনেস সক্ষমতা | ব্যবসা ঠিক *কোন* সমস্যাটি সমাধান করতে চায় এবং ব্যবসার মূল আর্থিক মূল্য কোথায় রয়েছে তা বোঝা। |
| **সলিউশন স্পেস (Solution Space)** | সফটওয়্যার আর্কিটেকচার ও বাস্তবায়ন | বাউন্ডেড কনটেক্সট (Bounded Contexts), ইউবিকুইটাস ল্যাঙ্গুয়েজ, ট্যাকটিক্যাল প্যাটার্নস (Aggregates, Services) | কোডের ভেতরে সফটওয়্যার কীভাবে মডেলিং ও বাউন্ডারি দিয়ে তৈরি ও বাস্তবায়িত হবে তা নির্ধারণ করা। |

### কখন DDD ব্যবহার করবেন এবং কখন ব্যবহার করবেন না (Decision Matrix)

```
                            জটিলতা মূল্যায়ন (Complexity Evaluation)
                                       │
                ┌──────────────────────┴──────────────────────┐
                ▼                                             ▼
     কম বিজনেস জটিলতা (Low Complexity)              উচ্চ বিজনেস জটিলতা (High Complexity)
   (CRUD, সাধারণ রিপোর্টিং, অ্যাডমিন প্যানেল)          (জটিল ইনভ্যারিয়্যান্ট, দ্রুত পরিবর্তনশীল নিয়ম)
                │                                             │
                ▼                                             ▼
          DDD ব্যবহার করবেন না                           DDD ব্যবহার করবেন
   • ট্রানজ্যাকশন স্ক্রিপ্ট / অ্যাক্টিভ রেকর্ড             • স্ট্র্যাটেজিক: বাউন্ডেড কনটেক্সট
   • দ্রুত প্রোটোটাইপিং / সরল MVC                        • ট্যাকটিক্যাল: রিচ ডোমেন মডেল
   • অতিরিক্ত কোড ও খরচের তুলনায় লাভ কম                 • ডোমেন মডেলিংয়ে উচ্চ ROI ও দীর্ঘস্থায়িত্ব
```

- **কখন DDD ব্যবহার করবেন**:
  - উচ্চ বিজনেস জটিলতা, যেখানে প্রচুর জটিল এবং পরিবর্তনশীল বিজনেস রুলস ও ইনভ্যারিয়্যান্টস (Invariants) বিদ্যমান।
  - বড় ক্রস-ফাংশনাল দল, যাদের একে অপরকে ব্লক না করে স্বাধীনভাবে কাজ করার জন্য সুস্পষ্ট বাউন্ডারি দরকার।
  - দীর্ঘমেয়াদী এন্টারপ্রাইজ প্ল্যাটফর্ম, যেখানে সফটওয়্যারের ডোমেন মডেলটি কোম্পানির মূল প্রতিদ্বন্দিতাপূর্ণ সুবিধা (Competitive Advantage)।
- **কখন DDD ব্যবহার করবেন না**:
  - সাধারণ CRUD (Create, Read, Update, Delete) অ্যাপ্লিকেশন বা ডাটা এন্ট্রি পোর্টাল।
  - প্রযুক্তি-প্রধান অ্যাপ্লিকেশন (যেমন: ডেটা পাইপলাইন/ETL, লগ প্রসেসিং, সাধারণ ইউটিলিটি মাইক্রোসার্ভিস)।
  - প্রাথমিক পর্যায়ের স্বল্পমেয়াদী MVP (যেমন ২ সপ্তাহের প্রুফ-অব-কনসেপ্ট), যেখানে ব্যবসার প্রকৃত ডোমেন এখনও সম্পূর্ণরূপে অনির্ধারিত।

---

## ২. আর্কিটেকচারাল কনসেপ্ট: অ্যানিমিক ডোমেন মডেল বনাম রিচ ডোমেন মডেল (Anemic vs. Rich Domain Model)

একটি **অ্যানিমিক ডোমেন মডেল (Anemic Domain Model)** (মার্টিন ফাউলারের চিহ্নিত অ্যান্টি-প্যাটার্ন) তখন তৈরি হয় যখন ডোমেন ক্লাসগুলোতে শুধুমাত্র প্রোপার্টি (Getter ও Setter) থাকে এবং কোনো বিজনেস আচরণ (Behavior) থাকে না; ফলে সমস্ত লজিক বাইরের সার্ভিস ক্লাসে ছড়িয়ে ছিটিয়ে থাকে। অপরদিকে, একটি **রিচ ডোমেন মডেল (Rich Domain Model)** ডাটার সাথে সেই ডাটা পরিবর্তনকারী বিজনেস নিয়মাবলী ও স্টেট ট্রানজিশনকে শক্তভাবে এনক্যাপসুলেট করে।

### রিচ ডোমেন মডেলের সুফল (Benefits)
- **কেন্দ্রীভূত ইনভ্যারিয়্যান্ট নিশ্চিতকরণ (Centralized Invariants)**: বিজনেস রুলস বাইপাস করা অসম্ভব, কারণ স্টেট পরিবর্তন শুধুমাত্র ভ্যালিডেটেড ডোমেন মেথডের মাধ্যমেই সম্ভব।
- **উচ্চ সংহতি ও টেস্টেবিলিটি (High Cohesion & Testability)**: ডাটাবেস বা এক্সটার্নাল ফ্রেমওয়ার্কের কোনো নির্ভরতা ছাড়াই ডোমেন লজিক সম্পূর্ণ আইসোলেশনে ইউনিট টেস্ট করা যায়।
- **কোড ডুপ্লিকেশন প্রতিরোধ (Preventing Duplication)**: "অর্ডার শিপ হওয়ার পর বাতিল করা যাবে না"—এই ধরণের নিয়ম একাধিক কন্ট্রোলারে না ছড়িয়ে সরাসরি `Order.Cancel()` এর ভেতরে সংরক্ষিত থাকে।

### ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)
- **লার্নিং কার্ভ (Learning Curve)**: ডেভেলপারদের টেবিল-ড্রাইভেন ডেটাবেস-ফার্স্ট চিন্তাধারা থেকে অবজেক্ট বিহেভিয়ার-ফার্স্ট চিন্তাধারায় অভ্যস্ত হতে সময় লাগে।
- **ORM ম্যাপিং জটিলতা (ORM Mapping)**: প্রাইভেট ফিল্ড ও ব্যাকড কালেকশনকে রিলেশনাল টেবিলের কলামে ম্যাপ করতে EF Core-এর ফ্লুয়েন্ট এপিআই কনফিগারেশনের প্রয়োজন হয়।

### .NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ
- `Ardalis.GuardClauses`: ডোমেন মেথডের ভেতরে ডিফেন্সিভ আর্গুমেন্ট এবং ইনভ্যারিয়্যান্ট ভ্যালিডেশনের জন্য।
- `FluentValidation`: ডোমেন মডেলে পৌঁছানোর পূর্বে অ্যাপ্লিকেশন লেয়ার বা DTO বাউন্ডারিতে ভ্যালিডেশনের জন্য।
- `Microsoft.EntityFrameworkCore`: প্রাইভেট কনস্ট্রাক্টর, ব্যাকিং ফিল্ডস (Backing Fields) এবং ওউনড এনটিটিজের (Owned Entities) সরাসরি নেটিভ সাপোর্ট।

```csharp
// Production C# 12 / .NET Example: Anemic vs Rich Domain Model
using System;
using System.Collections.Generic;

// ❌ ANTI-PATTERN: Anemic Domain Model (শুধুমাত্র ডেটা কন্টেইনার, সমস্ত লজিক সার্ভিস লেয়ারে ছড়ানো)
public class AnemicOrder
{
    public Guid Id { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<AnemicOrderItem> Items { get; set; } = new();
}

public class AnemicOrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

// ---------------------------------------------------------------------------------

// ✅ BEST PRACTICE: Rich Domain Model (এনক্যাপসুলেশন, ইনভ্যারিয়্যান্ট ভ্যালিডেশন এবং বিজনেস মেথড)
public sealed class OrderId(Guid value)
{
    public Guid Value { get; } = value != Guid.Empty ? value : throw new ArgumentException("OrderId cannot be empty.");
}

public enum OrderStatus { Draft, Submitted, Shipped, Cancelled }

public sealed class RichOrder
{
    private readonly List<RichOrderItem> _items = new();

    public OrderId Id { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<RichOrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => CalculateTotal();

    // EF Core / ORM ডিসিরিয়ালাইজেশনের জন্য প্রাইভেট কনস্ট্রাক্টর
    private RichOrder() { }

    public RichOrder(OrderId id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Status = OrderStatus.Draft;
    }

    // বিজনেস মেথড যেখানে ইনভ্যারিয়্যান্ট যাচাই করা হয়
    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot add items to an order that is not in Draft state.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Price cannot be negative.");

        var existingItem = _items.Find(x => x.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            _items.Add(new RichOrderItem(productId, quantity, unitPrice));
        }
    }

    public void Submit()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be submitted.");

        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot submit an empty order.");

        Status = OrderStatus.Submitted;
    }

    private decimal CalculateTotal()
    {
        decimal sum = 0;
        foreach (var item in _items)
            sum += item.LineTotal;
        return sum;
    }
}

public sealed class RichOrderItem
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal => Quantity * UnitPrice;

    private RichOrderItem() { }

    internal RichOrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal void IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
    }
}
```

---

## ৩. স্ট্র্যাটেজিক ডিজাইন: এন্টারপ্রাইজ স্কেল পরিচালনা (Strategic Design)

স্ট্র্যাটেজিক ডিজাইন মূলত বিজনেস ল্যান্ডস্কেপ ম্যাপিং, টিমের কাজের সীমানা নির্ধারণ এবং বিভিন্ন সিস্টেমের মধ্যে যোগাযোগের চুক্তি (Contract) নিয়ে কাজ করে।

![Strategic DDD Architecture](images/ddd-strategic-design.svg)

### ৩.১ ইউবিকুইটাস ল্যাঙ্গুয়েজ (Ubiquitous Language - UL)
**ইউবিকুইটাস ল্যাঙ্গুয়েজ** হলো একটি অবিসংবাদিত, সুনির্দিষ্ট ও স্পষ্ট পরিভাষা যা ডোমেন এক্সপার্ট (বিজনেস অ্যানালিস্ট, প্রোডাক্ট ম্যানেজার) এবং সফটওয়্যার ডেভেলপারদের মধ্যে হুবহু সমানভাবে ব্যবহৃত হয়।
- এটি একটি নির্দিষ্ট **বাউন্ডেড কনটেক্সট (Bounded Context)**-এর সীমানার মধ্যে কঠোরভাবে প্রযোজ্য।
- কোডের ভেতরে এর সরাসরি প্রতিফলন ঘটে: ক্লাসের নাম, মেথডের নাম এবং ডোমেন ইভেন্টসে সরাসরি বিজনেস পরিভাষা ব্যবহৃত হয় (যেমন: `policy.Lapse()` এর বদলে `policy.Status = 2` লেখা নিষিদ্ধ)।
- ডোমেন এক্সপার্টরা যদি "DTO", "Foreign Key" বা "Record" শব্দগুলো ব্যবহার না করেন, তবে ডোমেন মডেলে এই শব্দগুলো কখনোই থাকবে না।

### ৩.২ ডোমেন ও সাবডোমেন (Domains & Subdomains)
প্রতিটি এন্টারপ্রাইজ বিজনেস ডোমেন মূলত তিনটি স্বতন্ত্র সাবডোমেনে বিভক্ত:

1. **কোর ডোমেন (Core Domain)**:
   - ব্যবসার মূল আর্থিক উৎস এবং প্রতিযোগিতামূলক সুবিধা (Competitive Advantage)।
   - উদাহরণ: অ্যামাজনের রেকমেন্ডেশন ইঞ্জিন; উবারের ডাইনামিক প্রাইসিং ও ডিসপ্যাচিং।
   - কৌশল: নিজস্ব সেরা ইঞ্জিনিয়ারদের দিয়ে ইন-হাউস তৈরি করা এবং সম্পূর্ণ DDD অনুসরণ করা।
2. **সাপোর্টিং সাবডোমেন (Supporting Subdomain)**:
   - মূল ডোমেনকে সচল রাখতে প্রয়োজনীয় কার্যক্রম, কিন্তু এটি ব্যবসার মূল পার্থক্যকারী বিষয় নয়।
   - উদাহরণ: ই-কমার্স ওয়্যারহাউসে ইনভেন্টরি ট্র্যাকিং এবং পিকিং স্লিপ তৈরি।
   - কৌশল: নিজস্ব হালকা সার্ভিস হিসেবে তৈরি করা অথবা আউটসোর্স করা।
3. **জেনেরিক সাবডোমেন (Generic Subdomain)**:
   - প্রতিটি ব্যবসায় বিদ্যমান সাধারণ চাহিদা, যার কোনো প্রতিযোগিতামূলক বিশেষত্ব নেই।
   - উদাহরণ: ইউজার অথেন্টিকেশন/আইডেন্টিটি, পেমেন্ট গেটওয়ে প্রসেসিং, ইমেইল নোটিফিকেশন।
   - কৌশল: রেডিমেড থার্ড-পার্টি SaaS বা ওপেন-সোর্স লাইব্রেরি (যেমন: Stripe, SendGrid, Keycloak) কেনা।

### ৩.৩ বাউন্ডেড কনটেক্সট (Bounded Contexts)
একটি **বাউন্ডেড কনটেক্সট** হলো এমন একটি স্পষ্ট সীমানা যার মধ্যে একটি নির্দিষ্ট ডোমেন মডেল প্রযোজ্য থাকে। এই সীমানার ভেতর ইউবিকুইটাস ল্যাঙ্গুয়েজের প্রতিটি শব্দের একটাই অর্থ থাকে।

> **বাস্তব উদাহরণ**: `Account` শব্দটি
> - **Sales Context**-এ: `Account` হলো একজন সম্ভাব্য গ্রাহক (Lead) যার যোগাযোগের তথ্য ও পাইপলাইন স্ট্যাটাস থাকে।
> - **Billing Context**-এ: `Account` হলো একটি ইনভয়েসিং এনটিটি যার ট্যাক্স নম্বর এবং ক্রেডিট কার্ড তথ্য থাকে।
> - **Identity Context**-এ: `Account` হলো একটি লগইন ক্রেডেনশিয়াল যার পাসওয়ার্ড হ্যাশ এবং 2FA সিক্রেট থাকে।
>
> এগুলোকে একটিমাত্র ঢাউস "Universal Customer Account" ক্লাসে মেলানোর চেষ্টা করলে একটি বিশ্রী God Object তৈরি হয়। DDD এদেরকে পৃথক বাউন্ডেড কনটেক্সটে আলাদা রাখার সমাধান দেয়।

### ৩.৪ কনটেক্সট ম্যাপিং রিলেশনশিপ (Context Mapping Relationships)
একাধিক বাউন্ডেড কনটেক্সট যখন যোগাযোগ করে, তখন তাদের সম্পর্ক সুস্পষ্টভাবে চিহ্নিত করা আবশ্যক:

| সম্পর্কের ধরন | বিবরণ | স্বায়ত্তশাসন ও নির্ভরতা (Autonomy) |
| :--- | :--- | :--- |
| **শেয়ার্ড কার্নেল (Shared Kernel)** | দুই দল ডোমেন মডেল ও ডেটাবেস টেবিলের কিছু অংশ যৌথভাবে ভাগাভাগি করে। | উচ্চ কাপলিং; যে কোনো পরিবর্তনে উভয় দলের সম্মতি প্রয়োজন। |
| **কাস্টমার / সাপ্লায়ার (Customer / Supplier)** | আপস্ট্রিম (Supplier) দল ডাউনস্ট্রিম (Customer) দলের চাহিদামতো ফিচার সরবরাহ করে। | ডাউনস্ট্রিম দল আপস্ট্রিমের ডেলিভারি শিডিউলের ওপর নির্ভরশীল। |
| **কনফর্মিস্ট (Conformist)** | ডাউনস্ট্রিম দল কোনো অনুবাদ বা ফিল্টারিং ছাড়াই আপস্ট্রিমের মডেল সরাসরি গ্রহণ করে। | আপস্ট্রিমের যেকোনো পরিবর্তনের কাছে ডাউনস্ট্রিম দল জিম্মি থাকে। |
| **পার্টনার (Partner)** | দুটি কনটেক্সটের মধ্যে পারস্পরিক নির্ভরতা থাকে; রিলিজ ও পরিবর্তনের বিষয়ে ঘনিষ্ঠভাবে সমন্বয় করে। | উভয় দলের গভীর সমন্বয় প্রয়োজন। |
| **সেপারেট ওয়েজ (Separate Ways)** | দুই কনটেক্সটের মধ্যে ইন্টিগ্রেশনের খরচ লাভের চেয়ে বেশি মনে হলে তারা সম্পূর্ণ স্বাধীন সমাধান বেছে নেয়। | সম্পূর্ণ শূন্য কাপলিং। |
| **ওপেন হোস্ট সার্ভিস (OHS) / পাবলিশড ল্যাঙ্গুয়েজ (PL)** | আপস্ট্রিম একটি সুনির্দিষ্ট, স্থিতিশীল পাবলিক এপিআই (REST/gRPC) এবং স্ট্যান্ডার্ড স্কিমা (JSON/Protobuf) উন্মুক্ত করে। | চমৎকার ডিকাপলিং; আপস্ট্রিম ভার্সনিং নিয়ন্ত্রণ করে। |
| **অ্যান্টি-করাপশন লেয়ার (ACL)** | একটি অনুবাদক স্তর যা বহিরাগত বা পুরনো সিস্টেমের নোংরা মডেল থেকে ডাউনস্ট্রিম মডেলকে সুরক্ষিত রাখে। | ডাউনস্ট্রিম দলের সর্বোচ্চ স্বাধীনতা ও বিশুদ্ধতা রক্ষা পায়। |

### ৩.৫ স্ট্র্যাটেজিক ডোমেন ডিসকভারি: ইভেন্টস্টর্মিং ও ডোমেন স্টোরিটেলিং

ডোমেন ডিসকভারি হলো এমন একটি সহযোগিতামূলক প্রক্রিয়া যার মাধ্যমে ডোমেন বিশেষজ্ঞ এবং সফটওয়্যার ইঞ্জিনিয়াররা একসাথে বসে সমস্যার ক্ষেত্র অন্বেষণ করেন এবং সাবডোমেন, বাউন্ডেড কনটেক্সট ও অ্যাগ্রিগেট চিহ্নিত করেন।

![EventStorming Domain Discovery](images/event-storming-discovery.svg)

#### ইভেন্টস্টর্মিং স্টিকি-নোট ট্যাক্সোনমি (Alberto Brandolini)
ইভেন্টস্টর্মিং প্রক্রিয়ায় একটি বিশাল বোর্ডের উপর বাম থেকে ডানে **সময় প্রবাহের (Time Flow)** ক্রমানুসারে নির্দিষ্ট রঙের স্টিকি নোট সাজানো হয়:

| উপাদান (Artifact) | স্টিকি নোটের রঙ | অর্থ ও উদ্দেশ্য | বাস্তব উদাহরণ |
| :--- | :--- | :--- | :--- |
| **ডোমেন ইভেন্ট (Domain Event)** | 🟧 কমলা (Orange) | অতীতে ঘটে যাওয়া একটি গুরুত্বপূর্ণ ব্যবসায়িক সত্য (অতীতকালের ক্রিয়াপদ)। | `OrderPlaced`, `PaymentReceived`, `PackageDispatched` |
| **কমান্ড (Command)** | 🟦 নীল (Blue) | ব্যবহারকারী বা সিস্টেম দ্বারা সূচিত কোনো নির্দেশ বা অনুরোধ (বর্তমানকালের অনুজ্ঞা)। | `SubmitOrder`, `CancelSubscription`, `AuthorizeCard` |
| **অ্যাগ্রিগেট রুট (Aggregate Root)** | 🟨 হলুদ (Yellow) | ব্যবসায়িক সত্ত্বা এবং ইনভ্যারিয়্যান্ট বাউন্ডারি যা কমান্ড গ্রহণ করে এবং ইভেন্ট তৈরি করে। | `Order`, `CustomerProfile`, `Shipment` |
| **পলিসি / রিঅ্যাকশন (Policy)** | 🟪 বেগুনি (Purple) | স্বয়ংক্রিয় ব্যবসায়িক প্রতিক্রিয়া: *"যখনই [Event], তখনই [Command]"*। | *"যখনই OrderPlaced, তখনই ReserveInventory"* |
| **রিড মডেল (Read Model)** | 🟩 সবুজ (Green) | ব্যবহারকারীর সিদ্ধান্ত নেওয়ার জন্য স্ক্রিনে প্রদর্শিত ডেটা ভিউ বা প্রজেকশন। | `OrderSummaryScreen`, `AvailableInventoryDashboard` |
| **ইউজার / অ্যাক্টর (User / Actor)** | 🟨 ছোট হলুদ | যে মানব ব্যবহারকারী কমান্ডটি কার্যকর করছেন। | `Customer`, `WarehouseClerk`, `FraudAnalyst` |
| **এক্সটার্নাল সিস্টেম (External System)** | 🌸 গোলাপি (Pink) | তৃতীয় পক্ষের বহিরাগত সিস্টেম বা লিগ্যাসি প্ল্যাটফর্ম। | `Stripe Gateway`, `SAP ERP`, `SendGrid Email API` |

#### ইভেন্টস্টর্মিং-এর তিনটি স্তর:
1. **বিগ পিকচার (Big Picture)**: পুরো প্রতিষ্ঠানের সার্বিক ভ্যালু চেইন অনুসন্ধান করা। সাবডোমেন এবং প্রাকৃতিক বাউন্ডারি আবিষ্কার করা।
2. **প্রসেস মডেলিং (Process Modeling)**: একটি নির্দিষ্ট পূর্ণাঙ্গ ওয়ার্কফ্লোকে জুম-ইন করে দেখা (যেমন চেকআউট থেকে ডেলিভারি)।
3. **সফটওয়্যার ডিজাইন (Software Design)**: নির্দিষ্ট অ্যাগ্রিগেট বাউন্ডারি, ট্রানজ্যাকশন স্কোপ এবং বাউন্ডেড কনটেক্সট চূড়ান্ত করা।

---

## ৪. আর্কিটেকচারাল কনসেপ্ট: অ্যান্টি-করাপশন লেয়ার (Anti-Corruption Layer - ACL)

একটি **অ্যান্টি-করাপশন লেয়ার (ACL)** হলো দুটি ভিন্ন ডোমেন মডেলের মধ্যবর্তী একটি অনুবাদক স্তর। এটি বহিরাগত বা পুরনো লিগ্যাসি সিস্টেমের ত্রুটিপূর্ণ আর্কিটেকচার, অপ্রাসঙ্গিক পরিভাষা বা স্কিমা পরিবর্তনকে আপনার পরিষ্কার ডোমেন মডেলে ঢুকতে বাধা দেয়।

### অ্যান্টি-করাপশন লেয়ারের সুফল (Benefits)
- **ডোমেনের বিশুদ্ধতা (Domain Purity)**: আপনার কোর ডোমেন মডেলটি বহিরাগত সিস্টেমের নোংরা বা ক্ষতিকর স্কিমা পরিবর্তন থেকে সম্পূর্ণ নিরাপদ থাকে।
- **স্বাধীন বিবর্তন (Decoupled Evolution)**: বহিরাগত সিস্টেম পরিবর্তিত হলে, পুনর্নির্মাণ করা হলে বা প্রতিস্থাপিত হলে (যেমন Stripe বদলে Adyen ব্যবহার করলে) কোর ডোমেন লজিকে কোনো হাত দিতে হয় না।
- **চুক্তির দৃঢ়তা (Contract Enforcement)**: সিস্টেমের অভ্যন্তরে নিজস্ব ইউবিকুইটাস ল্যাঙ্গুয়েজ কঠোরভাবে বজায় রাখা সম্ভব হয়।

### ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)
- **অনুবাদের বাড়তি ওভারহেড**: অতিরিক্ত অনুবাদক ক্লাস (Adapters, Mappers) তৈরি করতে হয় এবং সামান্য মেমরি ও সিপিইউ প্রসেসিং প্রয়োজন হয়।
- **মডেলের ডুপ্লিকেশন**: বহিরাগত DTO এবং অভ্যন্তরীণ ডোমেন মডেলের আলাদা ডেটা ক্লাস সংরক্ষণ করতে হয়।

### .NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ
- `Mapster` / `AutoMapper`: দ্রুত ও কার্যকরভাবে অবজেক্ট ম্যাপিং সম্পাদনের জন্য।
- `Microsoft.Extensions.Http.Resilience` / `Polly`: এক্সটার্নাল রিকোয়েস্টে সার্কিট ব্রেকার ও রিট্রাই পলিসির জন্য।
- `Refit`: টাইপ-সেফ REST এপিআই ক্লায়েন্ট তৈরি করার জন্য।

```csharp
// Production C# 12 / .NET Example: Anti-Corruption Layer (ACL)
using System;
using System.Threading;
using System.Threading.Tasks;

// ১. এক্সটার্নাল লিগ্যাসি ভেন্ডর পেলোড (অপ্রাসঙ্গিক পরিভাষা ও কাঠামো)
public sealed record ExternalLegacyPaymentResponse(
    string tx_id,
    long amt_in_cents,
    string curr,
    string stat_code, // "00" = OK, "05" = Declined, "99" = Error
    string err_msg
);

// ২. অভ্যন্তরীণ বিশুদ্ধ ডোমেন মডেল (Ubiquitous Language)
public readonly record struct TransactionReference(string Value);
public enum PaymentExecutionStatus { Succeeded, Failed, Declined }

public sealed record DomainPaymentResult(
    TransactionReference Reference,
    decimal Amount,
    string Currency,
    PaymentExecutionStatus Status,
    string Reason
);

// ৩. ACL ইন্টারফেস (ডোমেন বা অ্যাপ্লিকেশন লেয়ারে সংজ্ঞায়িত)
public interface IPaymentGatewayAcl
{
    Task<DomainPaymentResult> ProcessChargeAsync(decimal amount, string currency, CancellationToken ct = default);
}

// ৪. ACL ইমপ্লিমেন্টেশন (ইনফ্রাস্ট্রাকচার লেয়ার - বহিরাগত নোংরামি ডোমেনে ঢুকতে বাধা দেয়)
public sealed class LegacyPaymentGatewayAcl : IPaymentGatewayAcl
{
    private readonly HttpClient _httpClient;

    public LegacyPaymentGatewayAcl(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DomainPaymentResult> ProcessChargeAsync(decimal amount, string currency, CancellationToken ct = default)
    {
        var vendorPayload = new
        {
            amt_in_cents = (long)(amount * 100),
            curr = currency.ToUpperInvariant()
        };

        // সিমুলেটেড এক্সটার্নাল কল
        var externalResponse = new ExternalLegacyPaymentResponse(
            tx_id: "TXN_LEGACY_98471204",
            amt_in_cents: (long)(amount * 100),
            curr: currency,
            stat_code: "00",
            err_msg: string.Empty
        );

        // অনুবাদ ও ডোমেন সুরক্ষা:
        var internalStatus = externalResponse.stat_code switch
        {
            "00" => PaymentExecutionStatus.Succeeded,
            "05" => PaymentExecutionStatus.Declined,
            _ => PaymentExecutionStatus.Failed
        };

        return new DomainPaymentResult(
            Reference: new TransactionReference(externalResponse.tx_id),
            Amount: externalResponse.amt_in_cents / 100m,
            Currency: externalResponse.curr,
            Status: internalStatus,
            Reason: externalResponse.err_msg
        );
    }
}
```

---

## ৫. ট্যাকটিক্যাল ডিজাইন: মূল গাঠনিক উপাদান ও ক্লিন আর্কিটেকচার (Tactical Design)

ট্যাকটিক্যাল DDD মূলত ক্লিন আর্কিটেকচারের ভেতরে ডোমেন লজিককে কোড-লেভেলে সাজানোর ব্যবহারিক ব্লক সরবরাহ করে।

![Tactical DDD Building Blocks](images/ddd-tactical-patterns.svg)

### ৫.১ ভ্যালু অবজেক্টস ও স্ট্রংলি-টাইপড আইডি (Value Objects & Strongly-Typed IDs)

একটি **ভ্যালু অবজেক্ট (Value Object)** হলো এমন একটি ইমিউটেবল অবজেক্ট যার নিজস্ব কোনো পরিচয় বা আইডি থাকে না; এর প্রোপার্টিগুলোর সম্মিলিত মানের দ্বারাই এর সমতা নির্ধারিত হয়। দুটি ভ্যালু অবজেক্টের সকল মান হুবহু এক হলে তারা সম্পূর্ণ সমান।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: ভ্যালু অবজেক্টস
- **সুফল (Benefits)**:
  - **Primitive Obsession দূর করে**: ভুল করে `Guid orderId`-র জায়গায় `Guid customerId` পাস করা প্রতিরোধ করে।
  - তৈরি হওয়ার সাথে সাথেই ভ্যালিডেশন নিশ্চিত করে এবং অবজেক্টকে অপরিবর্তনীয় (Immutable) রাখে।
  - সাইড-ইফেক্ট মুক্ত: মান পরিবর্তন করলে সবসময় একটি নতুন অবজেক্ট রিটার্ন হয়।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - ডাটাবেসের টেবিলে সাধারণ কলাম হিসেবে সংরক্ষণ করতে EF Core-এ অতিরিক্ত কনফিগারেশন (`OwnsOne` / `ComplexType`) করতে হয়।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - C# `record` এবং `record struct`: বিল্ট-ইন ভ্যালু-বেসড সমতা এবং ইমিউটেবিলিটি সাপোর্ট করে।
  - `Vogen` / `StronglyTypedId`: কোনো অতিরিক্ত মেমরি ওভারহেড ছাড়া স্ট্রংলি-টাইপড আইডি জেনারেট করে।

---

### ৫.২ এনটিটি, অ্যাগ্রিগেট এবং অ্যাগ্রিগেট রুট (Entities, Aggregates & Aggregate Roots)

একটি **এনটিটি (Entity)**-র নিজস্ব স্বতন্ত্র স্থায়ী পরিচয় (Identity) থাকে যা সময়ের সাথে সাথে এর বৈশিষ্ট্যের পরিবর্তন হলেও অক্ষুণ্ণ থাকে।

একটি **অ্যাগ্রিগেট (Aggregate)** হলো সম্পর্কিত এনটিটি এবং ভ্যালু অবজেক্টের একটি সুসংবদ্ধ গুচ্ছ যা ডেটা পরিবর্তনের ক্ষেত্রে একটি অবিভাজ্য একক বা ইউনিট হিসেবে বিবেচিত হয়। **অ্যাগ্রিগেট রুট (AR)** হলো এই গুচ্ছের প্রধান অভিভাবক এনটিটি যা বাইরের যেকোনো প্রবেশাধিকার নিয়ন্ত্রণ করে:

#### ভন ভার্নন (Vaughn Vernon)-এর অ্যাগ্রিগেট ডিজাইনের ৪টি মৌলিক নিয়ম:
1. **সুনির্দিষ্ট সীমানার ভেতরে ট্রু ইনভ্যারিয়্যান্ট মডেল করুন**: একটি ট্রানজ্যাকশন সীমানার মধ্যে প্রতিটি বিজনেস নিয়ম সব সময় সত্য ও সুরক্ষিত থাকতে হবে।
2. **ক্ষুদ্র অ্যাগ্রিগেট তৈরি করুন (Small Aggregates)**: শত শত চাইল্ড অবজেক্ট সহ বিশালাকার অ্যাগ্রিগেট পরিহার করুন; ক্ষুদ্র অ্যাগ্রিগেট ডেটাবেসের কনকারেন্সি সমস্যা দূর করে ও পারফরম্যান্স বাড়ায়।
3. **অন্যান্য অ্যাগ্রিগেটকে কেবল আইডির মাধ্যমে রেফারেন্স করুন**: কখনোই অন্য কোনো অ্যাগ্রিগেট রুটের অবজেক্ট রেফারেন্স সরাসরি ধরে রাখবেন না (যেমন: `Order` এর ভেতর `CustomerId` থাকবে, পুরো `Customer` অবজেক্ট নয়)।
4. **ইভেনচুয়াল কনসিস্টেন্সির মাধ্যমে অন্যান্য অ্যাগ্রিগেট আপডেট করুন**: একটি ডেটাবেস ট্রানজ্যাকশনে **কেবলমাত্র একটি অ্যাগ্রিগেট** পরিবর্তিত হওয়া উচিত। অন্যান্য অ্যাগ্রিগেট ডোমেন ইভেন্টের মাধ্যমে অ্যাসিনক্রোনাসভাবে আপডেট হবে।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: অ্যাগ্রিগেট রুট
- **সুফল (Benefits)**:
  - অভ্যন্তরীণ ডেটা ও চাইল্ড কালেকশন সুরক্ষিত থাকে, বাইরে থেকে কেউ সরাসরি পরিবর্তন করতে পারে না।
  - ট্রানজ্যাকশন বাউন্ডারি স্পষ্ট করায় ডেটাবেসের ডেডলক ও রেস কন্ডিশন প্রতিরোধ করে।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - একই সাথে একাধিক অ্যাগ্রিগেটকে একটিমাত্র এসকিউএল ট্রানজ্যাকশনে সংরক্ষণ করা নিয়মবিরুদ্ধ।
  - সিস্টেমগুলোর মধ্যে সমন্বয়ের জন্য ইভেনচুয়াল কনসিস্টেন্সি ও আউটবক্স প্যাটার্নের প্রয়োজন হয়।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - `Microsoft.EntityFrameworkCore`: প্রাইভেট ব্যাকিং ফিল্ডসে সরাসরি ডেটা ম্যাপ করার জন্য।
  - `Ardalis.GuardClauses`: ডোমেন ইনভ্যারিয়্যান্ট ভ্যালিডেশনের জন্য।

```csharp
// Production C# 12 / .NET Example: Value Objects, Strongly-Typed IDs & Aggregate Root
using System;
using System.Collections.Generic;

// ১. C# record struct দ্বারা তৈরি স্ট্রংলি-টাইপড আইডি
public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
    public static CustomerId Empty => new(Guid.Empty);
}

// ২. ভ্যালু অবজেক্ট: Money (ইমিউটেবল ও সেলফ-ভ্যালিডেটিং)
public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Money amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException("Currency must be a valid 3-letter ISO code.", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "USD") => new(0m, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add currency {other.Currency} to {Currency}.");
        return new Money(Amount + other.Amount, Currency);
    }
}

// ৩. ডোমেন ইভেন্ট ইন্টারফেস
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}

public sealed record OrderPlacedDomainEvent(Guid OrderId, CustomerId CustomerId, Money Total) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

// ৪. অ্যাগ্রিগেট রুট বেস ক্লাস (ইভেন্ট ট্র্যাকিং সহ)
public abstract class AggregateRoot<TId>
{
    public TId Id { get; protected set; } = default!;
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

// ৫. অ্যাগ্রিগেট রুট: Order
public sealed class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = new();

    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public bool IsCancelled { get; private set; }

    // EF Core-এর জন্য প্রাইভেট কনস্ট্রাক্টর
    private Order() { }

    public Order(Guid id, CustomerId customerId, string currency)
    {
        Id = id;
        CustomerId = customerId;
        TotalAmount = Money.Zero(currency);
        IsCancelled = false;
    }

    // ইনভ্যারিয়্যান্ট সুরক্ষিত স্টেট পরিবর্তন মেথড
    public void AddOrderItem(Guid productId, int quantity, Money unitPrice)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled order.");

        if (_items.Count >= 50)
            throw new InvalidOperationException("Order cannot contain more than 50 distinct items.");

        _items.Add(new OrderItem(Guid.NewGuid(), productId, quantity, unitPrice));
        TotalAmount = TotalAmount.Add(new Money(unitPrice.Amount * quantity, unitPrice.Currency));

        // ডোমেন ইভেন্ট রেইজ করা
        RaiseDomainEvent(new OrderPlacedDomainEvent(Id, CustomerId, TotalAmount));
    }

    public void Cancel()
    {
        if (IsCancelled) return;
        IsCancelled = true;
    }
}

// অ্যাগ্রিগেট বাউন্ডারির ভেতরের চাইল্ড এনটিটি (বাইরে থেকে সরাসরি অ্যাক্সেস নিষিদ্ধ)
public sealed class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }

    private OrderItem() { }

    internal OrderItem(Guid id, Guid productId, int quantity, Money price)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
}
```

---

### ৫.৩ ডিডিডি-তে রিপোজিটরি প্যাটার্ন: শুধুমাত্র অ্যাগ্রিগেট রুট এবং কালেকশন সিমান্টিকস (Repositories in DDD)

ডোমেন-ড্রিভেন ডিজাইনে **রিপোজিটরি (Repository)** ডাটাবেস পারসিস্টেন্সের উপর একটি কালেকশন-সদৃশ বিমূর্ততা (Abstraction) তৈরি করে। এটি একটি ইন-মেমরি কালেকশনের মতো আচরণ করে (`Add`, `GetById`, `Remove`) যাতে ডোমেন এবং অ্যাপ্লিকেশন লেয়ার কোনো নির্দিষ্ট এসকিউএল বা ওআরএম-এর উপর নির্ভরশীল না হয়।

#### ডিডিডি রিপোজিটরির অলঙ্ঘনীয় নিয়ম:
> [!IMPORTANT]
> **রিপোজিটরি শুধুমাত্র এবং শুধুমাত্র অ্যাগ্রিগেট রুটের (Aggregate Root) জন্যই তৈরি হবে।**
> সিস্টেমে `IOrderRepository` থাকবে, কিন্তু কোনো অবস্থাতেই **`IOrderItemRepository` তৈরি করা যাবে না**! চাইল্ড এনটিটি ও ভ্যালু অবজেক্ট শুধুমাত্র তাদের মূল অ্যাগ্রিগেট রুটের মাধ্যমেই সংরক্ষিত ও পরিবর্তিত হতে হবে। এটি অমান্য করলে অ্যাগ্রিগেটের ইনভ্যারিয়্যান্ট ভেঙে যায়।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: ডিডিডি রিপোজিটরি
- **সুফল (Benefits)**:
  - **ডিকাপলড পারসিস্টেন্স**: ডোমেন লেয়ারে ইন্টারফেস সংজ্ঞায়িত থাকে (`IOrderRepository`); ইনফ্রাস্ট্রাকচার লেয়ারে EF Core দিয়ে বাস্তবায়ন করা হয়।
  - **ইনভ্যারিয়্যান্টের নিরাপত্তা**: বাইরের কোনো কোড সরাসরি চাইল্ড এনটিটি সংরক্ষণ করতে পারে না, ফলে অ্যাগ্রিগেটের ভেতরের নিয়ম সব সময় অটুট থাকে।
  - **টেস্টেবিলিটি**: কোনো আসল ডাটাবেস ছাড়াই ইন-মেমরি কালেকশন মক করে ইউনিট টেস্ট করা যায়।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - সাধারণ রিড-অনলি রিপোর্টিংয়ের জন্য পুরো অ্যাগ্রিগেট লোড করা অপ্রয়োজনীয় মেমরি ব্যবহার ঘটায় (CQRS রিড পাথ দ্বারা এটি সমাধান করা হয়)।
  - রিপোজিটরি থেকে `IQueryable<T>` রিটার্ন করা একটি অ্যান্টি-প্যাটার্ন; এটি ওআরএম-এর লিক তৈরি করে।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - `Ardalis.Specification` / `Ardalis.Specification.EntityFrameworkCore`: ডোমেন কুয়েরি স্পেসিফিকেশন।
  - `Microsoft.EntityFrameworkCore`: স্ট্যান্ডার্ড ডাটাবেস এক্সেস লেয়ার।

```csharp
// Production C# 12 / .NET Example: DDD Repository Pattern
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// ১. শুধুমাত্র অ্যাগ্রিগেট রুটের জন্য জেনেরিক রিপোজিটরি ইন্টারফেস
public interface IRepository<T, TId> where T : AggregateRoot<TId>
{
    Task<T?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task AddAsync(T aggregate, CancellationToken ct = default);
    Task UpdateAsync(T aggregate, CancellationToken ct = default);
    Task DeleteAsync(T aggregate, CancellationToken ct = default);
}

// ২. ডোমেন লেয়ারের অ্যাগ্রিগেট-নির্দিষ্ট রিপোজিটরি ইন্টারফেস
public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order?> GetWithItemsByIdAsync(Guid id, CancellationToken ct = default);
}

// ৩. ইনফ্রাস্ট্রাকচার লেয়ারের বাস্তবায়ন (EF Core)
public sealed class OrderRepository : IOrderRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Order> _orders;

    public OrderRepository(DbContext context)
    {
        _context = context;
        _orders = context.Set<Order>();
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _orders.FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<Order?> GetWithItemsByIdAsync(Guid id, CancellationToken ct = default)
    {
        // অ্যাগ্রিগেট বাউন্ডারির ভেতরের চাইল্ড কালেকশন সহ লোড করা
        return await _orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task AddAsync(Order aggregate, CancellationToken ct = default)
    {
        await _orders.AddAsync(aggregate, ct);
    }

    public Task UpdateAsync(Order aggregate, CancellationToken ct = default)
    {
        _orders.Update(aggregate);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Order aggregate, CancellationToken ct = default)
    {
        _orders.Remove(aggregate);
        return Task.CompletedTask;
    }
}
```

---

### ৫.৪ ফ্যাক্টরি প্যাটার্ন: প্রারম্ভিক সৃষ্টি বনাম পুনর্গঠন (Factories: Initial Creation vs. Reconstitution)

ডিডিডি-তে একটি **ফ্যাক্টরি (Factory)** জটিল অ্যাগ্রিগেট ও এনটিটি তৈরির প্রক্রিয়াকে এনক্যাপসুলেট করে। কোনো অ্যাগ্রিগেট তৈরিতে যখন একাধিক নিয়ম, ক্যালকুলেশন বা অন্যান্য তথ্যের দরকার হয়, তখন ফ্যাক্টরি নিশ্চিত করে যে অবজেক্টটি একটি ১০০% ভ্যালিড অবস্থায় জন্ম নিচ্ছে।

#### প্রারম্ভিক সৃষ্টি বনাম পুনর্গঠনের মধ্যকার পার্থক্য (The Critical Difference):
- **প্রারম্ভিক সৃষ্টি (Initial Creation)**:
  - অ্যাগ্রিগেটটি এই প্রথমবার নতুনভাবে তৈরি হচ্ছে।
  - প্রাথমিক সমস্ত নিয়ম এবং ইনভ্যারিয়্যান্ট যাচাই করতে হবে।
  - ডিফল্ট টাইমস্ট্যাম্প, নতুন আইডি এবং প্রাথমিক স্ট্যাটাস সেট করতে হবে।
  - **প্রাথমিক ডোমেন ইভেন্ট রেইজ করবে** (যেমন: `OrderPlacedDomainEvent`) যাতে অন্যান্য সিস্টেম প্রতিক্রিয়া জানাতে পারে।
- **পুনর্গঠন (Reconstitution)**:
  - অ্যাগ্রিগেটটি ডাটাবেস বা ইভেন্ট স্ট্রিম থেকে মেমরিতে পুনরায় লোড করা হচ্ছে।
  - এটি অতীতে আগেই তৈরি ও যাচাই করা হয়েছে!
  - **কোনো অবস্থাতেই পুরানো ডোমেন ইভেন্ট নতুন করে রেইজ করা যাবে না** (করলে ডাটাবেস থেকে রিড করার সাথে সাথে আবার ভুয়া ইমেইল চলে যাবে বা কাস্টমারের কার্ড দুবার চার্জ হবে)।
  - কোনো প্রাথমিক ভ্যালিডেশন পুনরায় রান করা যাবে না যা সময়ের ব্যবধানে বিজনেস নিয়ম পরিবর্তনের কারণে ব্যর্থ হতে পারে।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: ডিডিডি ফ্যাক্টরি
- **সুফল (Benefits)**:
  - কোনো অ্যাগ্রিগেট কখনোই অসম্পূর্ণ বা অবৈধ অবস্থায় মেমরিতে জন্ম নিতে পারে না।
  - জটিল অবজেক্ট তৈরির দায়িত্ব কনস্ট্রাক্টর থেকে আলাদা থাকে।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - অতি সাধারণ অবজেক্টের জন্য আলাদা ফ্যাক্টরি ক্লাস তৈরি করা অপ্রয়োজনীয় জটিলতা বাড়াতে পারে।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - সাধারণ ক্ষেত্রে স্ট্যাটিক ফ্যাক্টরি মেথড (`Order.Create(...)`)।
  - বাহ্যিক ডিপেনডেন্সি (যেমন ক্লক বা জেনারেটর) থাকলে ডেডিকেটেড ফ্যাক্টরি ক্লাস।

```csharp
// Production C# 12 / .NET Example: Factory Creation vs Reconstitution
using System;
using System.Collections.Generic;

public sealed class CustomerOrderFactory
{
    private readonly TimeProvider _timeProvider;

    public CustomerOrderFactory(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    // ১. প্রারম্ভিক সৃষ্টি: ইনভ্যারিয়্যান্ট ভ্যালিডেশন করে এবং ডোমেন ইভেন্ট তৈরি করে
    public Order CreateNewOrder(CustomerId customerId, string currency, List<(Guid ProductId, int Qty, Money Price)> initialItems)
    {
        if (initialItems == null || initialItems.Count == 0)
            throw new InvalidOperationException("Cannot create an order without at least one item.");

        var orderId = Guid.NewGuid();
        var order = new Order(orderId, customerId, currency);

        foreach (var item in initialItems)
        {
            order.AddOrderItem(item.ProductId, item.Qty, item.Price);
        }

        return order;
    }

    // ২. পুনর্গঠন: ডাটাবেস থেকে লোড করার সময় কোনো নতুন ইভেন্ট তৈরি করবে না
    public static Order Reconstitute(
        Guid id, 
        CustomerId customerId, 
        Money totalAmount, 
        bool isCancelled)
    {
        var order = new Order(id, customerId, totalAmount.Currency);
        // রিফ্লেকশন বা ইন্টারনাল মেথড দিয়ে সংরক্ষিত মান বসানো হয় (ইভেন্ট ফায়ার করা ছাড়া)
        return order;
    }
}
```

---

### ৫.৫ স্পেসিফিকেশন প্যাটার্ন: ডোমেন প্রেডিকেট এনক্যাপসুলেশন (Specification Pattern)

**স্পেসিফিকেশন প্যাটার্ন (Specification Pattern)** বিজনেস নিয়ম, ফিল্টারিং শর্ত এবং বুলিয়ান লজিককে ফার্স্ট-ক্লাস ডোমেন অবজেক্ট হিসেবে এনক্যাপসুলেট করে।

#### ডিডিডি স্পেসিফিকেশনের তিনটি প্রধান ব্যবহার:
1. **ইন-মেমরি ভ্যালিডেশন**: কোনো একটি নির্দিষ্ট এনটিটি নিয়মের শর্ত পূরণ করে কিনা তা পরীক্ষা করা (`spec.IsSatisfiedBy(order)`).
2. **ইন-মেমরি কালেকশন ফিল্টারিং**: মেমরিতে থাকা লিস্ট থেকে শর্ত অনুযায়ী আইটেম আলাদা করা (`orders.Where(spec.ToFunc())`).
3. **ডাটাবেস কুয়েরি জেনারেশন**: LINQ এক্সপ্রেশন ট্রি তৈরি করা (`spec.ToExpression()`) যা EF Core সরাসরি এসকিউএল কুয়েরিতে রূপান্তর করে।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: স্পেসিফিকেশন প্যাটার্ন
- **সুফল (Benefits)**:
  - **DRY বিজনেস লজিক**: "কোন কাস্টমার ভিআইপি কাস্টমার?" এই ধরণের শর্ত একাধিক সার্ভিসে কপি-পেস্ট না করে একক স্পেসিফিকেশনে সংজ্ঞায়িত করা যায়।
  - **কম্পোজিশন**: `.And()`, `.Or()`, এবং `.Not()` দিয়ে একাধিক স্পেসিফিকেশন সহজে জোড়া লাগানো যায়।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - সাধারণ কুয়েরির জন্য স্পেসিফিকেশন তৈরি করলে অপ্রয়োজনীয় ক্লাস সংখ্যা বৃদ্ধি পায়।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - `Ardalis.Specification`: .NET ইকোসিস্টেমের সবচেয়ে জনপ্রিয় স্পেসিফিকেশন লাইব্রেরি।

```csharp
// Production C# 12 / .NET Example: Specification Pattern
using System;
using System.Linq.Expressions;

// ১. স্পেসিফিকেশন ইন্টারফেস ও বেস ক্লাস
public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
    Expression<Func<T, bool>> ToExpression();
}

public abstract class CompositeSpecification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    public ISpecification<T> And(ISpecification<T> other) => new AndSpecification<T>(this, other);
}

// ২. হাই-ভ্যালু অর্ডার স্পেসিফিকেশন (নিয়ম: বাতিল নয় এবং মূল্য >= ১০০০)
public sealed class HighValueOrderSpecification : CompositeSpecification<Order>
{
    private readonly decimal _threshold;

    public HighValueOrderSpecification(decimal threshold = 1000m)
    {
        _threshold = threshold;
    }

    public override Expression<Func<Order, bool>> ToExpression()
    {
        return order => !order.IsCancelled && order.TotalAmount.Amount >= _threshold;
    }
}

// ৩. কম্পোজিট AND স্পেসিফিকেশন
public sealed class AndSpecification<T> : CompositeSpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.AndAlso(
            Expression.Invoke(leftExpr, parameter),
            Expression.Invoke(rightExpr, parameter)
        );

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}
```

---

## ৬. আর্কিটেকচারাল কনসেপ্ট: ডোমেন ইভেন্টস ও ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন (Domain Events & Outbox)

একটি **ডোমেন ইভেন্ট (Domain Event)** ডোমেনের ভেতরে অতীতে ঘটে যাওয়া একটি গুরুত্বপূর্ণ বিজনেস পরিবর্তন বা ঘটনাকে রেকর্ড করে। এটি অপরিবর্তনীয় (Immutable) এবং অতীতকালের ক্রিয়াপদে নামাঙ্কিত হয় (যেমন: `OrderPlacedEvent`, `PaymentFailedEvent`)।

![Aggregate Boundary & Outbox Lifecycle](images/aggregate-boundary-lifecycle.svg)

### ইন-প্রসেস ডোমেন ইভেন্টস বনাম ইন্টিগ্রেশন ইভেন্টস
- **ডোমেন ইভেন্টস**: বাউন্ডেড কনটেক্সটের নিজস্ব সীমানার অভ্যন্তরীণ ঘটনা। একই অ্যাপ্লিকেশনের ভেতরে MediatR দিয়ে সিঙ্ক্রোনাসভাবে বা ইন-মেমরিতে হ্যান্ডল করা হয়।
- **ইন্টিগ্রেশন ইভেন্টস**: অন্যান্য বাউন্ডেড কনটেক্সট বা মাইক্রোসার্ভিসকে অবহিত করার জন্য মেসেজ ব্রোকারের (RabbitMQ, Kafka, Azure Service Bus) মাধ্যমে নেটওয়ার্ক অতিক্রম করে পাঠানো হয়।

### ডুয়াল-রাইট প্রবলেম এবং ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন
ডেটাবেসে ডেটা সেভ করার পর যদি মেসেজ ব্রোকারে ইভেন্ট পাবলিশ করার সময় সার্ভার ক্র্যাশ করে, তবে ডেটা ও মেসেজিংয়ের মধ্যে অমিল (Dual-Write Failure) তৈরি হয়। এই সমস্যা নিরসনে **ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন (Transactional Outbox Pattern)** ডেটাবেসের লোকাল ACID ট্রানজ্যাকশনের ভেতরেই একই সাথে মূল এনটিটি এবং ইভেন্টগুলোকে একটি `OutboxMessages` টেবিলে সেভ করে।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: ট্রানজ্যাকশনাল আউটবক্স
- **সুফল (Benefits)**:
  - **গ্যারান্টেড অ্যাট-লিস্ট-ওয়ান্স ডেলিভারি**: ব্রোকার ডাউন থাকলেও কোনো মেসেজ কখনো হারিয়ে যায় না।
  - কোনো ডিস্ট্রিবিউটেড 2-ফেজ কমিট (2PC) বা এক্সটার্নাল লক ছাড়াই কাজ করে।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - কনজিউমারদের **ইডেমপোটেন্সি (Idempotency)** বজায় রাখতে হয়, কারণ নেটওয়ার্ক রিট্রাইয়ের ফলে ডুপ্লিকেট মেসেজ যেতে পারে।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - `MassTransit` / `MassTransit.EntityFrameworkCoreIntegration`: বিল্ট-ইন আউটবক্স ও ইনবক্স প্যাটার্ন সাপোর্ট।
  - `MediatR`: ইন-প্রসেস ডোমেন ইভেন্ট ডিসপ্যাচের জন্য।
  - `Quartz.NET` / `BackgroundService`: আউটবক্স টেবিল থেকে মেসেজ ব্রোকারে পুশ করার ব্যাকগ্রাউন্ড ওয়ার্কার।

```csharp
// Production C# 12 / .NET Example: EF Core Interceptor for Outbox Pattern & Domain Events
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

// ১. আউটবক্স মেসেজ টেবিল এনটিটি
public sealed class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}

// ২. EF Core সেভ-চেঞ্জেস ইন্টারসেপ্টর: স্বয়ংক্রিয়ভাবে আউটবক্স টেবিল পপুলেট করে
public sealed class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var domainEvents = new List<IDomainEvent>();

        foreach (var entry in dbContext.ChangeTracker.Entries<AggregateRoot<Guid>>())
        {
            if (entry.Entity.DomainEvents.Count > 0)
            {
                domainEvents.AddRange(entry.Entity.DomainEvents);
                entry.Entity.ClearDomainEvents();
            }
        }

        // একই লোকাল ডেটাবেস ট্রানজ্যাকশনে আউটবক্স রো তৈরি করা
        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage
            {
                Type = domainEvent.GetType().Name,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                OccurredOnUtc = domainEvent.OccurredOnUtc
            };

            dbContext.Set<OutboxMessage>().Add(outboxMessage);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

---

## ৭. আর্কিটেকচারাল কনসেপ্ট: ডোমেন সার্ভিসেস বনাম অ্যাপ্লিকেশন সার্ভিসেস (Domain vs. Application Services)

| বৈশিষ্ট্য | ডোমেন সার্ভিস (Domain Service) | অ্যাপ্লিকেশন সার্ভিস (Application Service) | ইনফ্রাস্ট্রাকচার সার্ভিস (Infrastructure) |
| :--- | :--- | :--- | :--- |
| **আর্কিটেকচারাল লেয়ার** | ডোমেন লেয়ার (কোর) | অ্যাপ্লিকেশন লেয়ার | ইনফ্রাস্ট্রাকচার লেয়ার |
| **মূল উদ্দেশ্য** | একাধিক অ্যাগ্রিগেট জুড়ে থাকা পিওর বিজনেস লজিক। | ইউজ কেস ও ট্রানজ্যাকশন সমন্বয় করা। | আই/ও, ফাইল সিস্টেম, মেসেজ কিউ ও ডাটাবেস। |
| **ডিপেনডেন্সি** | শুধুমাত্র ডোমেন অবজেক্ট (বাইরের কোনো ডিপেনডেন্সি নেই)। | রিপোজিটরি, ডোমেন সার্ভিস, ইভেন্ট বাস। | থার্ড-পার্টি লাইব্রেরি, EF Core, SMTP, ক্লাউড SDK। |
| **স্টেট (State)** | স্টেটলেস (Stateless)। | স্টেটলেস (Stateless)। | কানেকশন হ্যান্ডেল সংরক্ষণ করে। |
| **বাস্তব উদাহরণ** | `CurrencyExchangeDomainService`, `OrderPricingService` | `CreateOrderCommandHandler`, `RegisterUserUseCase` | `SmtpEmailSender`, `BlobStorageService` |

### কখন একটি ডোমেন সার্ভিস তৈরি করবেন:
1. যখন কোনো অপারেশন গুরুত্বপূর্ণ বিজনেস হিসাব বা নিয়মাবলী ধারণ করে।
2. যখন অপারেশনটি একাধিক অ্যাগ্রিগেটের সমন্বয়ে ঘটে, যা একক কোনো অ্যাগ্রিগেটের মধ্যে রাখা দায়িত্ব লঙ্ঘন করে।
3. অপারেশনটি কোনোভাবেই অ্যাপ্লিকেশন সার্ভিসের বিষয় নয় (কারণ অ্যাপ্লিকেশন সার্ভিসে **কোনো বিজনেস রুলস থাকা সম্পূর্ণ নিষিদ্ধ**)।

#### .NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ
- `MediatR`: অ্যাপ্লিকেশন সার্ভিস হ্যান্ডলার (`IRequestHandler<TCommand, TResult>`) তৈরির জন্য।
- ডিপেনডেন্সি ইনজেকশন (`Microsoft.Extensions.DependencyInjection`): ডোমেন সার্ভিসগুলোকে Scoped বা Transient হিসেবে রেজিস্টার করা।

```csharp
// Production C# 12 / .NET Example: Domain Service vs. Application Service
using System;
using System.Threading;
using System.Threading.Tasks;

// ১. ডোমেন সার্ভিস (একাধিক অ্যাগ্রিগেট ব্যাপী পিওর বিজনেস লজিক)
public interface ICurrencyExchangeRateProvider
{
    decimal GetRate(string fromCurrency, string toCurrency);
}

public sealed class OrderPricingDomainService
{
    private readonly ICurrencyExchangeRateProvider _rateProvider;

    public OrderPricingDomainService(ICurrencyExchangeRateProvider rateProvider)
    {
        _rateProvider = rateProvider;
    }

    public Money CalculateDiscountedTotal(Order order, decimal customerVipDiscountPercent, string targetCurrency)
    {
        var rawTotal = order.TotalAmount;
        var discountMultiplier = 1m - (customerVipDiscountPercent / 100m);
        var discountedAmount = rawTotal.Amount * discountMultiplier;

        if (rawTotal.Currency == targetCurrency)
            return new Money(discountedAmount, targetCurrency);

        var exchangeRate = _rateProvider.GetRate(rawTotal.Currency, targetCurrency);
        return new Money(discountedAmount * exchangeRate, targetCurrency);
    }
}

// ২. অ্যাপ্লিকেশন সার্ভিস (ইউজ কেস অর্কেস্ট্রেশন, কোনো বিজনেস লজিক নেই)
public sealed record CreateOrderCommand(Guid CustomerId, string Currency) : MediatR.IRequest<Guid>;

public sealed class CreateOrderCommandHandler : MediatR.IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // ১. অ্যাগ্রিগেট রুট ইনিশিয়ালাইজ করা
        var order = new Order(Guid.NewGuid(), new CustomerId(request.CustomerId), request.Currency);

        // ২. রিপোজিটরির মাধ্যমে পারসিস্ট করা
        await _orderRepository.AddAsync(order, cancellationToken);

        // ৩. ইউনিট অব ওয়ার্ক কমিট করা
        await _unitOfWork.CommitAsync(cancellationToken);

        return order.Id;
    }
}

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct);
}
```

---

## ৮. অ্যাডভান্সড ট্যাকটিক্যাল প্যাটার্নস: ডোমেন পলিসি ও রেজাল্ট প্যাটার্ন (Domain Policy & Result Pattern)

### ৮.১ ডোমেন পলিসি / স্ট্র্যাটেজি প্যাটার্ন (Domain Policy Pattern)

ব্যবসায়িক ডোমেনে প্রায়শই পরিবর্তনশীল অ্যালগরিদম বা পলিসি থাকে (যেমন ডিসকাউন্ট হিসাব, ট্যাক্স রুলস)। এগুলো অ্যাগ্রিগেটের ভেতর হার্ডকোড করলে প্রচুর `if/else` জঞ্জাল তৈরি হয়। **পলিসি প্যাটার্ন** এই নিয়মগুলোকে ডোমেন স্ট্র্যাটেজি ইন্টারফেসে এনক্যাপসুলেট করে মেথড আর্গুমেন্ট হিসেবে গ্রহণ করে।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: ডোমেন পলিসি
- **সুফল (Benefits)**:
  - **Open/Closed Principle (OCP)**: অ্যাগ্রিগেটের কোনো কোড পরিবর্তন না করেই নতুন ডিসকাউন্ট নিয়ম বা বিজনেস পলিসি যোগ করা যায়।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - মেথডের সিগনেচারে পলিসি ইন্টারফেস পাস করতে হয়।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - স্ট্যান্ডার্ড C# স্ট্র্যাটেজি ইন্টারফেস ও ডিপেনডেন্সি ইনজেকশন।

```csharp
// Production C# 12 / .NET Example: Domain Policy Pattern
using System;

public interface IDiscountPolicy
{
    Money ApplyDiscount(Money originalPrice);
}

public sealed class VipCustomerDiscountPolicy : IDiscountPolicy
{
    public Money ApplyDiscount(Money originalPrice) =>
        new(originalPrice.Amount * 0.80m, originalPrice.Currency); // ২০% ছাড়
}

public sealed class RegularCustomerDiscountPolicy : IDiscountPolicy
{
    public Money ApplyDiscount(Money originalPrice) => originalPrice; // কোনো ছাড় নেই
}
```

---

### ৮.২ ডোমেন এরর হ্যান্ডলিং: রেজাল্ট প্যাটার্ন বনাম ডোমেন এক্সেপশন (Result Pattern vs. Domain Exceptions)

ঐতিহ্যবাহী অবজেক্ট ওরিয়েন্টেড কোডে প্রত্যাশিত বিজনেস নিয়মের ব্যত্যয় ঘটলে `throw new InvalidOperationException()` ছোঁড়া হয়। কিন্তু হাই-স্কেল সিস্টেমে এটি একটি আর্কিটেকচারাল অ্যান্টি-প্যাটার্ন:
- **পারফরম্যান্সের ক্ষতি**: এক্সেপশন তৈরির সময় সম্পূর্ণ স্ট্যাক ফ্রেম স্ক্যান করতে হয়, যা সাধারণ রিটার্নের তুলনায় হাজার গুণ বেশি সময় নষ্ট করে।
- **লুক্কায়িত কন্ট্রোল ফ্লো**: এক্সেপশন কোডের স্বাভাবিক প্রবাহ নষ্ট করে অদৃশ্য গোটু (goto)-র মতো কাজ করে।

**রেজাল্ট প্যাটার্ন (Result Pattern)** ডোমেন অপারেশনের ফলাফল হিসেবে একটি মান রিটার্ন করে যা হয় **সাফল্য (Success)** অথবা **ব্যবসায়িক ত্রুটি (DomainError)** নির্দেশ করে।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: রেজাল্ট প্যাটার্ন
- **সুফল (Benefits)**:
  - **সুস্পষ্ট মেথড সিগনেচার**: মেথড দেখলে বোঝা যায় এটি কী কী কারণে ব্যর্থ হতে পারে।
  - **রেলওয়ে ওরিয়েন্টেড প্রোগ্রামিং**: কোনো `try/catch` ছাড়াই অপারেশন সহজে চেইন করা যায়।
  - **উচ্চ পারফরম্যান্স**: শূন্য স্ট্যাক-ট্রেস ওভারহেড।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - কলার কোডে মান ব্যবহারের আগে `result.IsSuccess` চেক করতে হয়।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - `ErrorOr`: অত্যন্ত হালকা এবং ফ্লুয়েন্ট এরর-অর-ভ্যালু টাইপ।
  - `FluentResults`: বহুমুখী রেজাল্ট লাইব্রেরি।

```csharp
// Production C# 12 / .NET Example: Result Pattern with ErrorOr
using System;
using System.Collections.Generic;
using ErrorOr;

public static class DomainErrors
{
    public static class Order
    {
        public static Error EmptyCart => Error.Validation(
            code: "Order.EmptyCart", 
            description: "Cannot submit an order with zero items.");

        public static Error AlreadyCancelled => Error.Conflict(
            code: "Order.AlreadyCancelled", 
            description: "Cannot mutate an order that has already been cancelled.");
    }
}

public sealed class ResultOrder
{
    private readonly List<RichOrderItem> _items = new();
    public bool IsCancelled { get; private set; }

    public ErrorOr<Success> Cancel()
    {
        if (IsCancelled)
            return DomainErrors.Order.AlreadyCancelled;

        IsCancelled = true;
        return Result.Success;
    }
}
```

---

## ৯. আর্কিটেকচারাল সিনার্জি: CQRS, ইভেন্ট সোর্সিং এবং মাইক্রোসার্ভিসেস (Architectural Synergies)

### ৯.১ DDD + CQRS
কমান্ড ও কুয়েরির দায়িত্ব পৃথক করা:
- **Write Path (Commands)**: ইনভ্যারিয়্যান্ট নিশ্চিত করার জন্য সম্পূর্ণ অ্যাগ্রিগেট রুট লোড করে।
- **Read Path (Queries)**: পারফরম্যান্স বাড়াতে অ্যাগ্রিগেট বাইপাস করে সরাসরি Dapper বা এসকিউএল ভিউ দিয়ে ফ্ল্যাট DTO রিটার্ন করে।

```
       Write Path (DDD Aggregates)                     Read Path (Fast Projections)
  ┌───────────────────────────────────┐           ┌────────────────────────────────────┐
  │ Command -> Handler -> Aggregate   │           │ Query -> Dapper / Fast SQL View    │
  │ -> Invariants -> EF Core Save     │           │ -> Directly returns UI DTO         │
  └───────────────────────────────────┘           └────────────────────────────────────┘
                    │                                               ▲
                    ▼                                               │
             [Write Database] ─── (CDC / Sync / Projection) ─── [Read Store]
```

---

### ৯.২ DDD + ইভেন্ট সোর্সিং (Event Sourcing)
অ্যাগ্রিগেটের বর্তমান স্টেট টেবিলের রো হিসেবে সেভ না করে, অতীত থেকে বর্তমান পর্যন্ত ঘটে যাওয়া সমস্ত ডোমেন ইভেন্টের অবিচ্ছেদ্য স্ট্রিম ডাটাবেসে সেভ করা হয়। খালি অবজেক্টে শুরু থেকে সমস্ত ইভেন্ট রি-প্লে করে অ্যাগ্রিগেটের তাৎক্ষণিক অবস্থা তৈরি করা হয় (`state.Apply(event)`)।

---

### ৯.৩ DDD + মাইক্রোসার্ভিসেস বনাম মডুলার মনোলিথ

অনেকেই ভুলবশত মনে করেন **১টি বাউন্ডেড কনটেক্সট = ১টি মাইক্রোসার্ভিস**।

> [!WARNING]
> ডোমেনের সীমানা পরিপক্ক হওয়ার আগেই বাউন্ডেড কনটেক্সটগুলোকে আলাদা আলাদা মাইক্রোসার্ভিসে ভাগ করলে তৈরি হয় **ডিস্ট্রিবিউটেড মনোলিথ (Distributed Monolith)**—যেখানে নেটওয়ার্ক ল্যাটেন্সি ও ডিস্ট্রিবিউটেড ট্রানজ্যাকশনের ঝামেলা তৈরি হয় কিন্তু মাইক্রোসার্ভিসের কোনো আসল সুবিধা পাওয়া যায় না।

- **বেস্ট প্র্যাকটিস (মার্টিন ফাউলারের MonolithFirst)**:
  - প্রথমে একটি **মডুলার মনোলিথ (Modular Monolith)**-এর ভেতরে পৃথক C# প্রজেক্ট বা অ্যাসেম্বলি হিসেবে প্রতিটি বাউন্ডেড কনটেক্সট তৈরি করুন।
  - যখন সত্যিই স্বাধীন স্কেলিং বা আলাদা টিমের স্বাধীনতার প্রয়োজন হবে, কেবল তখনই সেই বাউন্ডেড কনটেক্সটকে পৃথক মাইক্রোসার্ভিসে রূপান্তর করুন।

---

## ১০. সমন্বিত স্টাফ ও লিড ইন্টারভিউ প্রশ্ন ও উত্তর (Staff-Level Interview Q&As)

### প্রশ্ন ১: ডোমেন-ড্রিভেন ডিজাইন কী এবং কখন এটি ব্যবহার করা একদম উচিত নয়?
**উত্তর**:
ডোমেন-ড্রিভেন ডিজাইন (DDD) হলো সফটওয়্যার আর্কিটেকচারের একটি দর্শন যা জটিল ব্যবসায়িক ডোমেন মডেলের সাথে সফটওয়্যার ডিজাইনকে সমন্বিত করে। এর দুটি মূল স্তম্ভ: **কৌশলগত ডিজাইন (Strategic Design)** এবং **ব্যবহারিক ডিজাইন (Tactical Design)**।

**কখন DDD ব্যবহার করা উচিত নয়**:
1. **সাধারণ CRUD অ্যাপ্লিকেশন**: যখন ব্যবসায়িক লজিক অতি সাধারণ (যেমন ডেটা এন্ট্রি, অ্যাডমিন পোর্টাল)।
2. **টেকনিক্যাল পাইপলাইন ও ডেটা ইনজেশন**: উচ্চ-গতির লগ প্রসেসিং বা ডেটা স্ট্রিমিং অ্যাপ্লিকেশনে অবজেক্ট ইনভ্যারিয়্যান্টের চেয়ে ডেটা পাইপলাইন ফ্রেমওয়ার্ক বেশি জরুরি।
3. **স্বল্পমেয়াদী প্রোটোটাইপ বা MVP**: দ্রুত প্রোটোটাইপের ক্ষেত্রে ট্রানজ্যাকশন স্ক্রিপ্ট বা অ্যাক্টিভ রেকর্ড প্যাটার্ন অনেক বেশি দ্রুত কাজ দেয়।

---

### প্রশ্ন ২: এনটিটি এবং ভ্যালু অবজেক্টের মধ্যকার মৌলিক পার্থক্য কী? একটি বাউন্ডেড কনটেক্সটের এনটিটি কি অন্য কনটেক্সটে ভ্যালু অবজেক্ট হতে পারে?
**উত্তর**:
- **এনটিটি**: স্থায়ী পরিচয় (`Id`) দ্বারা সংজ্ঞায়িত। সময়ের সাথে সাথে প্রোপার্টি পরিবর্তন হলেও এর আইডির কোনো পরিবর্তন হয় না।
- **ভ্যালু অবজেক্ট**: নিজস্ব কোনো আইডি নেই; এর সমস্ত বৈশিষ্ট্যের সম্মিলিত মানের দ্বারাই এর পরিচয় নির্ধারিত হয়। এটি কঠোরভাবে ইমিউটেবল।

**কনটেক্সট ভেদে রূপান্তর**:
**হ্যাঁ, অবশ্যই হতে পারে**। 
- **লজিস্টিকস বা শিপিং কনটেক্সটে**, `Address` একটি **এনটিটি** হতে পারে (যার AddressId, জিপিএস কোঅর্ডিনেট এবং ডেলিভারি বাধাগ্রস্ত কিনা তার স্ট্যাটাস থাকে)।
- **অর্ডার কনটেক্সটে**, `Address` হলো কেবল একটি **ভ্যালু অবজেক্ট** যা অর্ডার করার মুহূর্তে স্ন্যাপশট হিসেবে অর্ডারে জুড়ে দেওয়া হয়।

---

### প্রশ্ন ৩: অ্যাগ্রিগেট রুট কী এবং "প্রতি ট্রানজ্যাকশনে ১টি অ্যাগ্রিগেট" নিয়মটির তাৎপর্য কী? যদি দুটি অ্যাগ্রিগেট একসাথে আপডেট করতে হয় তবে কী করবেন?
**উত্তর**:
একটি **অ্যাগ্রিগেট** হলো ইনভ্যারিয়্যান্ট কনসিস্টেন্সি বাউন্ডারি। বাইরের যেকোনো যোগাযোগ শুধুমাত্র **অ্যাগ্রিগেট রুট (AR)**-এর মাধ্যমেই হতে হবে।

**প্রতি ট্রানজ্যাকশনে ১টি অ্যাগ্রিগেট নিয়ম**:
একটি ডাটাবেস ট্রানজ্যাকশনে কেবলমাত্র একটিমাত্র অ্যাগ্রিগেট পরিবর্তন করা উচিত। এর ফলে ট্রানজ্যাকশন দ্রুত শেষ হয় এবং লক কনটেনশন সর্বনিম্ন থাকে।

**দুটি অ্যাগ্রিগেট একসাথে আপডেট করার প্রয়োজন হলে**:
1. **ইভেনচুয়াল কনসিস্টেন্সি (প্রস্তাবিত)**: অ্যাগ্রিগেট A আপডেট হয়ে ডোমেন ইভেন্ট ছুঁড়ে দেবে এবং কমিট করবে। অ্যাসিনক্রোনাস ব্যাকগ্রাউন্ড হ্যান্ডলার ইভেন্টটি পেয়ে অ্যাগ্রিগেট B আপডেট করবে।
2. **বাউন্ডারি মূল্যায়ন**: যদি দুটি সত্ত্বাকে একই ট্রানজ্যাকশনে আপডেট না করলে বড় ব্যবসায়িক ক্ষতি হয়, তবে বুঝতে হবে তারা আসলে **একই অ্যাগ্রিগেটের অংশ**।

---

### প্রশ্ন ৪: রিপোজিটরি কেন শুধুমাত্র অ্যাগ্রিগেট রুটের জন্যই তৈরি হতে হবে? চাইল্ড এনটিটির জন্য রিপোজিটরি বানালে কী ক্ষতি হয়?
**উত্তর**:
অ্যাগ্রিগেটের ভেতরের যাবতীয় নিয়মকানুন ও ইনভ্যারিয়্যান্ট কেবলমাত্র অ্যাগ্রিগেট রুটের দ্বারাই নিয়ন্ত্রিত হয়।
যদি কোনো ডেভেলপার ভুলবশত `IOrderItemRepository` তৈরি করে চাইল্ড আইটেম সরাসরি আপডেট বা ডিলিট করেন:
1. মূল `Order` অ্যাগ্রিগেট রুটের কোড কখনোই রান হবে না, ফলে অর্ডারের মোট মূল্য বা সর্বোচ্চ আইটেম সংক্রান্ত নিয়ম বাইপাস হয়ে যাবে।
2. মেমরিতে বা ক্যাশে থাকা অভিভাবক অ্যাগ্রিগেট ডাটাবেসের ডেটার সাথে অসামঞ্জস্যপূর্ণ হয়ে পড়বে।
3. অ্যাগ্রিগেট রুটের কনকারেন্সি টোকেন আপডেট না হওয়ায় রেস কন্ডিশন তৈরি হবে।
অতএব, চাইল্ড এনটিটির সকল পরিবর্তন অ্যাগ্রিগেট রুটের মেথডের মাধ্যমে হতে হবে এবং অ্যাগ্রিগেট রুটের রিপোজিটরি দিয়েই সেভ হতে হবে।

---

### প্রশ্ন ৫: ডিডিডি ফ্যাক্টরিতে প্রারম্ভিক সৃষ্টি (Initial Creation) এবং পুনর্গঠনের (Reconstitution) মধ্যকার গুরুত্বপূর্ণ পার্থক্য কী?
**উত্তর**:
- **প্রারম্ভিক সৃষ্টি**: যখন কোনো অ্যাগ্রিগেট এই প্রথমবার তৈরি হচ্ছে। এটি সমস্ত ব্যবসায়িক শর্ত যাচাই করবে, নতুন আইডি দেবে এবং **প্রাথমিক ডোমেন ইভেন্ট তৈরি করবে** (যেমন `OrderPlacedDomainEvent`) যাতে আউটবক্স ও মেসেজ ব্রোকার দিয়ে অন্যান্য সিস্টেম কাজ শুরু করতে পারে।
- **পুনর্গঠন**: যখন ডাটাবেসের টেবিল বা ইভেন্ট স্ট্রিম থেকে আগে তৈরি হওয়া অ্যাগ্রিগেট মেমরিতে লোড করা হয়। পুনর্গঠনের সময় **কখনোই পুরানো ডোমেন ইভেন্ট রেইজ করা যাবে না**। রিড করার সাথে সাথে ইভেন্ট রেইজ হলে ভুয়া ডুপ্লিকেট ইমেইল বা দ্বিতীয়বার টাকা কাটার বিপর্যয় ঘটবে।

---

### প্রশ্ন ৬: ডিডিডি-তে স্পেসিফিকেশন প্যাটার্ন কী এবং এর তিনটি ব্যবহারিক ক্ষেত্র কী কী?
**উত্তর**:
**স্পেসিফিকেশন প্যাটার্ন** ডোমেনের ব্যবসায়িক শর্ত ও বুলিয়ান নিয়মাবলীকে ফার্স্ট-ক্লাস ক্লাসে এনক্যাপসুলেট করে।
এর তিনটি প্রধান ব্যবহার:
1. **ইন-মেমরি ভ্যালিডেশন**: কোনো এনটিটি শর্ত পূরণ করে কিনা তা যাচাই করা (`spec.IsSatisfiedBy(order)`).
2. **ইন-মেমরি কালেকশন ফিল্টারিং**: মেমরি লিস্ট থেকে আইটেম বাছাই করা (`orders.Where(spec.ToFunc())`).
3. **ডাটাবেস কুয়েরি জেনারেশন**: স্পেসিফিকেশনের এক্সপ্রেশন ট্রি ব্যবহার করে EF Core দিয়ে সরাসরি অপটিমাইজড এসকিউএল তৈরি করা (`spec.ToExpression()`).

---

### প্রশ্ন ৭: ইভেন্টস্টর্মিং (EventStorming) পদ্ধতিটি ব্যাখ্যা করুন। এর বিভিন্ন রঙের স্টিকি নোট কী নির্দেশ করে?
**উত্তর**:
**ইভেন্টস্টর্মিং** হলো আলবার্তো ব্র্যান্ডোলিনি আবিষ্কৃত একটি দ্রুত ও কার্যকর কোলাবোরেটিভ মডেলিং পদ্ধতি যা ডোমেন এক্সপার্ট এবং ইঞ্জিনিয়ারদের একসাথে নিয়ে জটিল ব্যবসায়িক ডোমেন বিশ্লেষণ করে।
স্টিকি নোটের রঙের তালিকা:
- **কমলা**: **ডোমেন ইভেন্ট** (অতীতে ঘটে যাওয়া ব্যবসায়িক সত্য, যেমন: `OrderPlaced`)।
- **নীল**: **কমান্ড** (ব্যবহারকারী বা সিস্টেমের অনুরোধ, যেমন: `SubmitOrder`)।
- **হলুদ**: **অ্যাগ্রিগেট রুট** (ব্যবসায়িক সীমানা যা কমান্ড গ্রহণ করে, যেমন: `Order`)।
- **বেগুনি**: **পলিসি / রিঅ্যাকশন** (*"যখনই [Event], তখনই [Command]"*)।
- **সবুজ**: **রিড মডেল** (স্ক্রিনের ডেটা ভিউ)।
- **ছোট হলুদ**: **ইউজার / রোল** (যে ব্যবহারকারী কমান্ড দিচ্ছে)।
- **গোলাপি**: **এক্সটার্নাল সিস্টেম** (থার্ড-পার্টি পেমেন্ট বা লিগ্যাসি এপিআই)।

---

### প্রশ্ন ৮: অ্যানিমিক ডোমেন মডেল কী, কেন এটি অ্যান্টি-প্যাটার্ন এবং C#-এ কীভাবে রিচ ডোমেন মডেলে রিফ্যাক্টর করবেন?
**উত্তর**:
অ্যানিমিক ডোমেন মডেলে ক্লাসের মধ্যে কেবল পাবলিক গেটার ও সেটার থাকে এবং কোনো নিয়মকানুন থাকে না। সমস্ত লজিক বাইরের সার্ভিস ক্লাসে ছড়িয়ে থাকে।
**কেন এটি অ্যান্টি-প্যাটার্ন**:
- অবজেক্ট ওরিয়েন্টেড এনক্যাপসুলেশন ভঙ্গ করে।
- ব্যবসায়িক নিয়মাবলী একাধিক কন্ট্রোলার ও সার্ভিসে ডুপ্লিকেট হয়।
- অবজেক্ট মেমরিতে সব সময় সঠিক অবস্থায় থাকে তা নিশ্চিত করা যায় না।

**C#-এ রিফ্যাক্টর করার উপায়**:
1. সমস্ত প্রোপার্টির সেটার `private` করুন।
2. পাবলিক প্যারামিটারহীন কনস্ট্রাক্টর বাদ দিয়ে প্যারামিটারাইজড কনস্ট্রাক্টর বা ফ্যাক্টরি মেথড ব্যবহার করুন।
3. প্রোপার্টি সেটারের বদলে অর্থবহ ডোমেন মেথড (`order.Cancel()`) তৈরি করুন।
4. চাইল্ড কালেকশনকে প্রাইভেট ফিল্ডে রেখে বাইরে `IReadOnlyCollection<T>` এক্সপোজ করুন।

---

### প্রশ্ন ৯: আধুনিক .NET ডিডিডি-তে বিজনেস ভ্যালিডেশন ব্যর্থতার জন্য এক্সেপশন ছোঁড়ার বদলে রেজাল্ট প্যাটার্ন (Result Pattern) কেন শ্রেয়?
**উত্তর**:
প্রত্যাশিত ব্যবসায়িক নিয়ম ভঙ্গের জন্য এক্সেপশন ছোঁড়ার সমস্যা:
1. **সিপিইউ পারফরম্যান্স ক্ষতি**: এক্সেপশন থ্রো করলে স্ট্যাক ফ্রেম স্ক্যান করার কারণে প্রচুর সিপিইউ সাইকেল নষ্ট হয়।
2. **লুক্কায়িত কোড প্রবাহ**: এক্সেপশন স্বাভাবিক কোড এক্সিকিউশন ব্যাহত করে।
3. **টাইপ সেফটি**: রেজাল্ট প্যাটার্ন (`ErrorOr<Order>` বা `Result<Order>`) ব্যবহার করলে মেথড সিগনেচারেই স্পষ্টভাবে দেখা যায় কাজটি কী কী কারণে ব্যর্থ হতে পারে, যা কম্পাইল টাইমে উভয় অবস্থা হ্যান্ডেল করতে বাধ্য করে।

---

### প্রশ্ন ১০: ডিস্ট্রিবিউটেড 2-ফেজ কমিট (2PC) ছাড়া ক্রস-অ্যাগ্রিগেট ইনভ্যারিয়্যান্ট কীভাবে পরিচালনা করবেন?
**উত্তর**:
1. **ডোমেন ইভেন্টস + সাগা প্যাটার্ন (Saga Pattern)**: অ্যাগ্রিগেট A পরিবর্তন হয়ে ইভেন্ট পাবলিশ করবে, সাগা সেই ইভেন্ট শুনে অ্যাগ্রিগেট B-কে কমান্ড পাঠাবে। ব্যর্থ হলে কমপেনসেটিং ট্রানজ্যাকশন চালাবে।
2. **রিজার্ভেশন প্যাটার্ন (Reservation Pattern)**: নির্দিষ্ট সময়ের জন্য রিসোর্স সাময়িকভাবে ব্লক (Reserve) করে রাখা।
3. **ইডেমপোটেন্ট কনজিউমার + ইনবক্স প্যাটার্ন**: ডুপ্লিকেট মেসেজ নিষ্ক্রিয় করার ব্যবস্থা রাখা।

---

### প্রশ্ন ১১: অ্যান্টি-করাপশন লেয়ার (ACL) কী? বাস্তব জীবনের একটি উদাহরণ দিন।
**উত্তর**:
একটি **অ্যান্টি-করাপশন লেয়ার (ACL)** হলো দুটি ভিন্ন সিস্টেমের মাঝখানের অনুবাদক স্তর যা লিগ্যাসি বা বহিরাগত সিস্টেমের খারাপ ডিজাইন থেকে অভ্যন্তরীণ ডোমেনকে রক্ষা করে।
**বাস্তব উদাহরণ**:
একটি আধুনিক ইকমার্স প্ল্যাটফর্ম যখন ২০ বছরের পুরনো SAP ERP সিস্টেমের সাথে যুক্ত হয়, তখন SAP-এর দুর্বোধ্য কোড (`VBELN`, `MATNR`) সরাসরি ডোমেনে না এনে একটি ACL অ্যাডাপ্টারের মাধ্যমে সেগুলোকে পরিষ্কার `ProductId` এবং `OrderReference`-এ রূপান্তর করা হয়।

---

### প্রশ্ন ১২: ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন কীভাবে ডুয়াল-রাইট সমস্যা সমাধান করে?
**উত্তর**:
যখন অ্যাগ্রিগেট পরিবর্তনের সাথে সাথে একই লোকাল এসকিউএল ট্রানজ্যাকশনের মধ্যে একটি `OutboxMessages` টেবিলে ইভেন্টের ডেটা সংরক্ষণ করা হয়। ডেটাবেস ক্র্যাশ করলে দুটোই রোলব্যাক হয়, আর সফল হলে দুটোই কমিট হয়। পরবর্তীতে ব্যাকগ্রাউন্ড ওয়ার্কার বা CDC টুল আউটবক্স টেবিল থেকে মেসেজ ব্রোকারে ইভেন্ট পাবলিশ করে এবং রো-টিকে প্রসেসড হিসেবে চিহ্নিত করে।

---

### প্রশ্ন ১৩: এনক্যাপসুলেশন না ভেঙে Entity Framework Core দিয়ে কীভাবে জটিল ডিডিডি অ্যাগ্রিগেট পারসিস্ট করবেন?
**উত্তর**:
1. **প্রাইভেট ব্যাকিং ফিল্ডস**: `builder.Property(x => x.Total).HasField("_total").UsePropertyAccessMode(PropertyAccessMode.Field);`
2. **প্রাইভেট কনস্ট্রাক্টর**: EF Core রিফ্লেকশনের মাধ্যমে প্রাইভেট কনস্ট্রাক্টর দিয়ে অবজেক্ট তৈরি করতে পারে।
3. **ওউনড এনটিটি ও ভ্যালু অবজেক্ট**: `builder.ComplexProperty(o => o.ShippingAddress)` ব্যবহার করা।
4. **ভ্যালু কনভার্টার**: স্ট্রংলি-টাইপড আইডির জন্য `HasConversion` ব্যবহার করা।

---

### প্রশ্ন ১৪: ডোমেন সার্ভিস, অ্যাপ্লিকেশন সার্ভিস এবং ইনফ্রাস্ট্রাকচার সার্ভিসের পার্থক্য কী?
**উত্তর**:
- **ডোমেন সার্ভিস**: একাধিক অ্যাগ্রিগেট ব্যাপী বিশুদ্ধ ব্যবসায়িক নিয়ম ও গণনা ধারণ করে; কোনো ডেটাবেস বা এক্সটার্নাল ডিপেনডেন্সি থাকে না (যেমন: `TaxCalculationDomainService`).
- **অ্যাপ্লিকেশন সার্ভিস**: ইউজ কেস সমন্বয় করে, রিপোজিটরি থেকে অ্যাগ্রিগেট লোড করে, ডোমেন মেথড কল করে এবং ট্রানজ্যাকশন কমিট করে; কোনো বিজনেস নিয়ম থাকে না (যেমন: `CheckoutOrderCommandHandler`).
- **ইনফ্রাস্ট্রাকচার সার্ভিস**: আই/ও এবং নেটওয়ার্ক প্রযুক্তিগত কাজ সম্পন্ন করে (যেমন: `SmtpEmailService`, `RedisCacheService`).

---

### প্রশ্ন ১৫: ফ্ল্যাশ সেল বা উচ্চ কনটেনশন অ্যাগ্রিগেটের ক্ষেত্রে ডিডিডি কীভাবে পরিচালনা করবেন?
**উত্তর**:
1. **অ্যাগ্রিগেট ডিকম্পোজিশন / স্প্লিটিং**: একটিমাত্র ইনভেন্টরি অ্যাগ্রিগেটে ১০,০০০ ব্যালেন্স না রেখে, স্টককে ছোট ছোট ব্লকে (যেমন ১০০টি ব্লকে ১০০টি করে) ভাগ করে রিকোয়েস্ট রাউট করা।
2. **টোকেন বাকেট প্যাটার্ন**: রেডিমেড ইন-মেমরি স্টোরে (যেমন Redis) অ্যাটমিক স্ক্রিপ্ট দিয়ে স্টক রিজার্ভ করে টোকেন তৈরি করা এবং পরে অ্যাসিনক্রোনাসভাবে অ্যাগ্রিগেটে অর্ডার প্রসেস করা।
3. **ইভেন্ট সোর্সিং**: রো-লেভেল লকিং এড়াতে কেবল `StockReservedEvent` যোগ করে যাওয়া।

---

### প্রশ্ন ১৬: প্রতিটি বাউন্ডেড কনটেক্সট কি আলাদা মাইক্রোসার্ভিস হতে হবে? মডুলার মনোলিথের ভূমিকা কী?
**উত্তর**:
**একদমই না**। বাউন্ডেড কনটেক্সট হলো একটি *লজিক্যাল ডোমেন বাউন্ডারি*, আর মাইক্রোসার্ভিস হলো একটি *ফিজিক্যাল ডিপ্লয়মেন্ট বাউন্ডারি*।
- একটি একক কোডবেসের ভেতরে **মডুলার মনোলিথ** হিসেবে একাধিক বাউন্ডেড কনটেক্সট পরিচালনা করা সবচেয়ে উত্তম কৌশল।
- মডুলার মনোলিথে কনটেক্সটগুলোর সীমানা সহজে রিফ্যাক্টর করা যায়, মেমরি কলের মাধ্যমে দ্রুত কাজ করে এবং ডিস্ট্রিবিউটেড সিস্টেমের জটিলতা থাকে না।
- শুধুমাত্র ভিন্ন স্কেলিং রিকোয়ারমেন্ট বা আলাদা টিমের স্বাধীনতার প্রয়োজন হলেই বাউন্ডেড কনটেক্সটকে মাইক্রোসার্ভিসে আলাদা করা উচিত।

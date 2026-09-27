# -*- coding: utf-8 -*-
"""
Bangla content for Domain-Driven Design (DDD) Documentation & Notebook.
Maintains 100% strict 1-to-1 parity with content_en.py.
Preserves core CS & architectural terms in English / transliterated format.
"""

BN_SECTIONS = []

# Section 1: Intro
BN_SECTIONS.append(("markdown", """# ডোমেন-ড্রিভেন ডিজাইন (Domain-Driven Design - DDD): মূলনীতি, আর্কিটেকচার এবং স্টাফ-লেভেল ইন্টারভিউ সহায়িকা

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
- `Microsoft.EntityFrameworkCore`: প্রাইভেট কনস্ট্রাক্টর, ব্যাকিং ফিল্ডস (Backing Fields) এবং ওউনড এনটিটিজের (Owned Entities) সরাসরি নেটিভ সাপোর্ট।"""))

BN_SECTIONS.append(("code", """// Production C# 12 / .NET Example: Anemic vs Rich Domain Model
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

    // EF Core এবং ওআরএম রিকনস্ট্রাকশনের জন্য প্রাইভেট কনস্ট্রাক্টর
    private RichOrder() { }

    public RichOrder(OrderId id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Status = OrderStatus.Draft;
    }

    // বিজনেস লজিক সম্বলিত মেথড - ইনভ্যারিয়্যান্ট সর্বদা সংরক্ষিত থাকে
    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("ড্রাফট অবস্থায় না থাকলে অর্ডারে আইটেম যোগ করা যাবে না।");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "পরিমাণ অবশ্যই শুন্যের চেয়ে বেশি হতে হবে।");

        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "দাম ঋণাত্মক হতে পারবে না।");

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
            throw new InvalidOperationException("শুধুমাত্র ড্রাফট অর্ডার সাবমিট করা সম্ভব।");

        if (_items.Count == 0)
            throw new InvalidOperationException("খালি অর্ডার সাবমিট করা যাবে না।");

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
}"""))

BN_SECTIONS.append(("markdown", """---

## ৩. স্ট্র্যাটেজিক ডিজাইন: এন্টারপ্রাইজ স্কেল পরিচালনা (Strategic Design)

স্ট্র্যাটেজিক ডিজাইন মূলত পুরো এন্টারপ্রাইজের বিজনেসের সীমানা নির্ধারণ, টিমগুলোর কাজের ক্ষেত্র স্পষ্টকরণ এবং সিস্টেমগুলোর মধ্যকার ইন্টারঅ্যাকশন কন্ট্রাক্ট তৈরি নিয়ে কাজ করে।

![Strategic DDD Architecture](images/ddd-strategic-design.svg)

### ৩.১ ইউবিকুইটাস ল্যাঙ্গুয়েজ (Ubiquitous Language - UL)
**ইউবিকুইটাস ল্যাঙ্গুয়েজ** হলো একটি সার্বজনীন, স্পষ্ট এবং দ্ব্যর্থহীন শব্দভাণ্ডার যা ডোমেন এক্সপার্ট (বিজনেস অ্যানালিস্ট, প্রোডাক্ট ম্যানেজার) এবং সফটওয়্যার ইঞ্জিনিয়ারদের মাঝে যৌথভাবে ব্যবহৃত হয়।
- এটি কঠোরভাবে একটি নির্দিষ্ট **বাউন্ডেড কনটেক্সটের** মধ্যে সীমাবদ্ধ।
- কোডের ভেতরেও এই ভাষা হুবহু প্রতিফলিত হয়: ক্লাসের নাম, মেথডের নাম এবং ডোমেন ইভেন্টগুলো বাস্তব বিজনেসের নামে নামকরণ করা হয় (যেমন: `policy.Status = 2` এর বদলে `policy.Lapse()`)।
- ডোমেন এক্সপার্টরা যদি "DTO", "Foreign Key", বা "Table" শব্দগুলো ব্যবহার না করেন, তবে সেই শব্দগুলো ডোমেন মডেলে স্থান পাবে না।

### ৩.২ ডোমেন ও সাবডোমেন (Domains & Subdomains)
প্রতিটি এন্টারপ্রাইজ বিজনেস ডোমেন মূলত তিনটি সাবডোমেনে বিভক্ত:

1. **কোর ডোমেন (Core Domain)**:
   - ব্যবসার প্রধান ডিফারেনশিয়েটর এবং প্রতিযোগিতামূলক সুবিধা যা কোম্পানিকে লাভজনক করে তোলে।
   - উদাহরণ: আমাজনের রেকমেন্ডেশন ইঞ্জিন; উবারের ডাইনামিক প্রাইসিং ও ডিসপ্যাচিং সিস্টেম।
   - কৌশল: নিজস্ব সেরা সফটওয়্যার ইঞ্জিনিয়ারদের দিয়ে সম্পূর্ণ DDD নীতি অনুসরণ করে কাস্টম তৈরি করা।
2. **সাপোর্টিং সাবডোমেন (Supporting Subdomain)**:
   - মূল কোর ডোমেনকে সফল করার জন্য প্রয়োজনীয় সহায়ক কার্যক্রম, তবে এটি ব্যবসার মূল ইউনিক ইউএসপি (USP) নয়।
   - উদাহরণ: ই-কমার্স ওয়্যারহাউসে ইনভেন্টরি ট্র্যাকিং বা পিকিং স্লিপ তৈরি।
   - কৌশল: ইন্টারনাল টিম দিয়ে সাধারণ ইঞ্জিনিয়ারিংয়ে তৈরি করা অথবা আউটসোর্স করা।
3. **জেনেরিক সাবডোমেন (Generic Subdomain)**:
   - স্ট্যান্ডার্ড বিজনেস প্রয়োজনীয়তা যা সব কোম্পানিতেই প্রায় একরকম থাকে এবং এতে কোনো স্বতন্ত্র ব্যবসায়িক মূল্য নেই।
   - উদাহরণ: আইডেন্টিটি/লগইন (Authentication), বিলিং/পেমেন্ট গেটওয়ে, ইমেইল নোটিফিকেশন।
   - কৌশল: তৈরি না করে সরাসরি বাজারের মানসম্মত SaaS বা ওপেন সোর্স লাইব্রেরি ব্যবহার করা (যেমন: Stripe, Keycloak, SendGrid)।

### ৩.৩ বাউন্ডেড কনটেক্সট (Bounded Context)
একটি **বাউন্ডেড কনটেক্সট** হলো এমন একটি নির্দিষ্ট সীমানা যার ভেতরে একটি নির্দিষ্ট ডোমেন মডেল সম্পূর্ণ বৈধ ও সামঞ্জস্যপূর্ণ থাকে। বাউন্ডারির ভেতরে ইউবিকুইটাস ল্যাঙ্গুয়েজের প্রতিটি শব্দের ঠিক একটি অর্থ থাকে।

> **বাস্তব উদাহরণ**: `Account` শব্দটি
> - **সেলস কনটেক্সটে (Sales Context)**: একজন সম্ভাব্য গ্রাহক যার সাথে যোগাযোগের তথ্য এবং সেলস পাইপলাইনের ধাপ যুক্ত।
> - **বিলিং কনটেক্সটে (Billing Context)**: একটি ইনভয়েস এনটিটি যার সাথে ট্যাক্স আইডি এবং পেমেন্ট ক্রেডিট কার্ড যুক্ত।
> - **অথেনটিকেশন কনটেক্সটে (Identity Context)**: একটি লগইন ক্রেডেনশিয়াল যার সাথে পাসওয়ার্ড হ্যাশ এবং 2FA সিক্রেট যুক্ত।
>
> এগুলোকে একত্রিত করে যদি একটি "Unified Customer Account" ক্লাস বানানো হয়, তবে শত শত নাল কলাম বিশিষ্ট একটি দানবীয় গড অবজেক্ট (God Object) তৈরি হয়। DDD এই সমস্যাটি পৃথক বাউন্ডেড কনটেক্সটে বিভক্ত করে স্থায়ীভাবে দূর করে।

### ৩.৪ কনটেক্সট ম্যাপিংয়ের সম্পর্কসমূহ (Context Mapping Relationships)
একাধিক বাউন্ডেড কনটেক্সট একে অপরের সাথে কীভাবে যুক্ত হবে তা স্পষ্টভাবে ম্যাপিং করতে হয়:

| সম্পর্ক (Relationship) | বিবরণ (Description) | অটোনমি ও কাপলিংয়ের প্রভাব |
| :--- | :--- | :--- |
| **শেয়ার্ড কার্নেল (Shared Kernel)** | দুই বা ততোধিক টিম ডোমেন মডেলের একটি সাধারণ অংশ এবং ডেটাবেস টেবিল শেয়ার করে। | উচ্চ কাপলিং; যেকোনো পরিবর্তনে উভয় টিমের সমঝোতার প্রয়োজন হয়। |
| **কাস্টমার / সাপ্লায়ার (Customer / Supplier)** | আপস্ট্রিম (সাপ্লায়ার) সিস্টেম ডাউনস্ট্রিম (কাস্টমার) সিস্টেমের প্রয়োজনীয় রিকোয়ারমেন্ট সরবরাহ করতে বাধ্য থাকে। | ডাউনস্ট্রিম আপস্ট্রিমের রিলিজ শিডিউলের ওপর নির্ভরশীল থাকে। |
| **কনফর্মিস্ট (Conformist)** | ডাউনস্ট্রিম কোনো ট্রান্সলেশন ছাড়াই আপস্ট্রিমের মডেল হুবহু মেনে নেয়। | আপস্ট্রিমের যেকোনো পরিবর্তনের ঝুঁকিতে থাকে ডাউনস্ট্রিম। |
| **ওপেন হোস্ট সার্ভিস (OHS) / পাবলিশড ল্যাঙ্গুয়েজ (PL)** | আপস্ট্রিম একটি স্থিতিশীল, ডকুমেন্টেড পাবলিক এপিআই (REST/gRPC) ও স্ট্যান্ডার্ড স্কিমা প্রদান করে। | উচ্চ ডিকাপলিং; আপস্ট্রিম ভার্সনিং নিয়ন্ত্রণ করে। |
| **অ্যান্টি-করাপশন লেয়ার (ACL)** | একটি আইসোলেশন লেয়ার যা ডাউনস্ট্রিমকে এক্সটার্নাল বা লেগ্যাসি মডেলের দূষণ থেকে রক্ষা করে। | ডাউনস্ট্রিমের স্বাধীনতা ও বিশুদ্ধতা সর্বোচ্চ সুরক্ষিত থাকে। |
| **সেপারেট ওয়েজ (Separate Ways)** | দুটি কনটেক্সট ইন্টিগ্রেশন থেকে তেমন কোনো বড় সুবিধা না পাওয়ায় সম্পূর্ণ আলাদাভাবে সমস্যা সমাধান করে। | শূন্য কাপলিং। |

---

## ৪. আর্কিটেকচারাল কনসেপ্ট: অ্যান্টি-করাপশন লেয়ার (Anti-Corruption Layer - ACL)

একটি **অ্যান্টি-করাপশন লেয়ার (ACL)** হলো একটি অনুবাদক ও আইসোলেশন মেকানিজম যা দুটি ভিন্ন ডোমেন মডেলের মাঝে অবস্থান করে। এটি বাইরের কোনো থার্ড-পার্টি সার্ভিস বা পুরোনো লেগ্যাসি সিস্টেমের ত্রুটিপূর্ণ মডেল যেন আপনার নিজস্ব ক্লিন ডোমেন মডেলকে দূষিত না করে তা নিশ্চিত করে।

### অ্যান্টি-করাপশন লেয়ারের সুফল (Benefits)
- **ডোমেন বিশুদ্ধতা (Domain Purity)**: আপনার ক্লিন ডোমেন মডেল বাইরের সিস্টেমে ঘটা যেকোনো ব্রেকিং পরিবর্তন থেকে সুরক্ষিত থাকে।
- **স্বাধীন বিবর্তন (Decoupled Evolution)**: ভেতরের বিজনেস লজিকে কোনো হাত না দিয়েই বাইরের পার্টনার পরিবর্তন করা যায় (যেমন: Stripe সরিয়ে Adyen বসানো)।
- **কন্ট্রাক্ট এনফোর্সমেন্ট (Contract Enforcement)**: ভেতরে আপনার নিজস্ব ইউবিকুইটাস ল্যাঙ্গুয়েজ বলবৎ থাকে এবং শুধু বাউন্ডারিতে ট্রান্সলেশন ঘটে।

### ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)
- **কোড ওভারহেড (Translation Overhead)**: অ্যাডাপ্টার, ম্যাপার ও ট্রান্সলেটর তৈরির জন্য অতিরিক্ত কোড লিখতে হয়।
- **মেমরি ও পারফরম্যান্স ল্যাটেন্সি**: প্রতিটি রিকোয়েস্টে অবজেক্ট ম্যাপিংয়ের জন্য সামান্য সিপিইউ ও মেমরি খরচ হয়।

### .NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ
- `Mapster` / `AutoMapper`: এক্সটার্নাল DTO এবং ইন্টারনাল ডোমেন মডেলের মধ্যে হাই-পারফরম্যান্স ম্যাপিং।
- `Microsoft.Extensions.Http.Resilience` / `Polly`: এক্সটার্নাল কলকে সুরক্ষিত রাখতে সার্কিট ব্রেকার ও রিট্রাই মেকানিজম।
- `Refit`: এক্সটার্নাল এপিআই ডাকার জন্য টাইপ-সেফ REST ক্লায়েন্ট লাইব্রেরি।"""))

BN_SECTIONS.append(("code", """// Production C# 12 / .NET Example: Anti-Corruption Layer (ACL)
using System;
using System.Threading;
using System.Threading.Tasks;

// ১. এক্সটার্নাল লেগ্যাসি ভেন্ডরের এপিআই রেসপন্স (জটিল, snake_case এবং অদ্ভুত ফিল্ড নাম)
public sealed record ExternalLegacyPaymentResponse(
    string tx_id,
    long amt_in_cents,
    string curr,
    string stat_code, // "00" = সফল, "05" = রিজেক্টেড, "99" = সিস্টেম এরর
    string err_msg
);

// ২. আমাদের ইন্টারনাল ক্লিন ডোমেন মডেল (Ubiquitous Language অনুযায়ী গঠিত)
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

// ৪. ACL বাস্তবায়ন (ইনফ্রাস্ট্রাকচার লেয়ার - বাইরের সিস্টেমের বিশৃঙ্খলা এখানেই আবদ্ধ থাকে)
public sealed class LegacyPaymentGatewayAcl : IPaymentGatewayAcl
{
    private readonly HttpClient _httpClient;

    public LegacyPaymentGatewayAcl(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DomainPaymentResult> ProcessChargeAsync(decimal amount, string currency, CancellationToken ct = default)
    {
        // ভেতরের ক্লিন রিকোয়েস্টকে বাইরের ভেন্ডরের ফরম্যাটে রূপান্তর
        var vendorPayload = new
        {
            amt_in_cents = (long)(amount * 100),
            curr = currency.ToUpperInvariant()
        };

        // সিমুলেটেড লেগ্যাসি কল (প্রোডাকশনে: await _httpClient.PostAsJsonAsync(...))
        var externalResponse = new ExternalLegacyPaymentResponse(
            tx_id: "TXN_LEGACY_98471204",
            amt_in_cents = (long)(amount * 100),
            curr: currency,
            stat_code: "00",
            err_msg: string.Empty
        );

        // ট্রান্সলেশন ও ডিফেন্স: বাইরের স্ট্যাটাস কোডকে ইন্টারনাল ডোমেন এনামে রূপান্তর
        var internalStatus = externalResponse.stat_code switch
        {
            "00" => PaymentExecutionStatus.Succeeded,
            "05" => PaymentExecutionStatus.Declined,
            _ => PaymentExecutionStatus.Failed
        };

        // ইন্টারনাল ডোমেন অবজেক্ট আকারে রিটার্ন
        return new DomainPaymentResult(
            Reference: new TransactionReference(externalResponse.tx_id),
            Amount: externalResponse.amt_in_cents / 100m,
            Currency: externalResponse.curr,
            Status: internalStatus,
            Reason: externalResponse.err_msg
        );
    }
}"""))

BN_SECTIONS.append(("markdown", """---

## ৫. ট্যাকটিক্যাল ডিজাইন: বিল্ডিং ব্লকস এবং ক্লিন আর্কিটেকচার (Tactical Design)

ট্যাকটিক্যাল DDD কোড লেভেলে বিভিন্ন গাঠনিক উপাদান সরবরাহ করে যার মাধ্যমে ডোমেন লজিককে লেয়ার্ড বা ক্লিন আর্কিটেকচারের ভেতরে সুশৃঙ্খল রাখা যায়।

![Tactical DDD Building Blocks](images/ddd-tactical-patterns.svg)

### ৫.১ ভ্যালু অবজেক্ট এবং স্ট্রংলি-টাইপড আইডি (Value Objects & Strongly-Typed IDs)

একটি **ভ্যালু অবজেক্ট (Value Object)** হলো এমন একটি অপরিবর্তনশীল (Immutable) অবজেক্ট যার নিজস্ব কোনো অনন্য আইডেন্টিটি নেই; বরং এটি তার বৈশিষ্ট্যের সমন্বয় দ্বারা সংজ্ঞায়িত হয়। দুটি ভ্যালু অবজেক্টের প্রতিটি ফিল্ড সমান হলে তারা সম্পূর্ণরূপে সমান বলে গণ্য হয়।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: ভ্যালু অবজেক্টস
- **সুফল (Benefits)**:
  - **প্রিমিটিভ অবসেশন (Primitive Obsession)** দূর করে (যেমন `Guid orderId` এর জায়গায় ভুলবশত `Guid customerId` পাস করা প্রতিরোধ করে)।
  - অবজেক্ট তৈরির সময়েই সেলফ-ভ্যালিডেশন নিশ্চিত করে এবং অবজেক্টকে অপরিবর্তনীয় রাখে।
  - সাইড-ইফেক্ট মুক্ত মেথড: ভ্যালু অবজেক্টে যেকোনো গাণিতিক অপারেশনের ফলাফল সর্বদা একটি নতুন ইনস্ট্যান্স রিটার্ন করে।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - রিলেশনাল ডাটাবেসে ম্যাপ করতে EF Core-এ `OwnsOne` বা `ComplexType` কনফিগার করতে হয়।
  - বারবার নতুন অবজেক্ট তৈরির ফলে মেমরিতে সাময়িক অবজেক্ট অ্যালোকেশনের ওভারহেড হতে পারে।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - C# `record` এবং `record struct`: বিল্ট-ইন ভ্যালু ইকুয়ালিটি এবং ইমিউটেবিলিটি সাপোর্ট।
  - `Vogen` বা `StronglyTypedId`: জিরো-ওভারহেড স্ট্রংলি-টাইপড আইডি তৈরির সোর্স জেনারেটর।

---

### ৫.২ এনটিটি, এগ্রিগেট এবং এগ্রিগেট রুট (Entities, Aggregates & Aggregate Roots)

একটি **এনটিটি (Entity)** হলো এমন একটি অবজেক্ট যার একটি সুনির্দিষ্ট ধারাবাহিক আইডেন্টিটি (`Id`) থাকে যা তার পুরো লাইফসাইকেল জুড়ে অপরিবর্তিত থাকে।

একটি **এগ্রিগেট (Aggregate)** হলো সম্পর্কিত এনটিটি এবং ভ্যালু অবজেক্টের একটি সুসংহত গুচ্ছ যাকে ডাটা পরিবর্তনের ক্ষেত্রে একটি একক ট্রানজ্যাকশন ইউনিট হিসেবে বিবেচনা করা হয়। এই গুচ্ছের প্রধান অভিভাবক বা একমাত্র এন্ট্রি পয়েন্ট হলো **এগ্রিগেট রুট (Aggregate Root)**:

#### ভন ভার্ননের এগ্রিগেট ডিজাইনের ৪টি মৌলিক নিয়ম (Vaughn Vernon's 4 Rules):
1. **প্রকৃত ইনভ্যারিয়্যান্টগুলোকে কনসিস্টেন্ট বাউন্ডারির মধ্যে মডেল করুন**: ইনভ্যারিয়্যান্ট হলো একটি বিজনেস নিয়ম যা এগ্রিগেটের ভেতর সর্বদা সত্য হতে হবে।
2. **ছোট আকারের এগ্রিগেট ডিজাইন করুন (Design Small Aggregates)**: শত শত চাইল্ড সম্বলিত বিশাল এগ্রিগেট বানাবেন না; ছোট এগ্রিগেটে ডাটাবেস কনকারেন্সি কনফ্লিক্ট কম হয় এবং থ্রুপুট বাড়ে।
3. **অন্যান্য এগ্রিগেটকে কেবল আইডির মাধ্যমে রেফারেন্স করুন**: কখনোই অন্য এগ্রিগেট রুটের ডিরেক্ট অবজেক্ট রেফারেন্স রাখবেন না (যেমন: `Order` এর মধ্যে থাকবে `CustomerId`, সরাসরি `Customer` ক্লাস নয়)।
4. **ইভেনচুয়াল কনসিস্টেন্সির মাধ্যমে অন্যান্য এগ্রিগেট আপডেট করুন**: একটি ডাটাবেস ট্রানজ্যাকশনে **শুধুমাত্র একটি এগ্রিগেট** আপডেট করবেন। অন্যান্য এগ্রিগেট ডোমেন ইভেন্টের মাধ্যমে অ্যাসিনক্রোনাসভাবে আপডেট হবে।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: এগ্রিগেট রুট
- **সুফল (Benefits)**:
  - অভ্যন্তরীণ চাইল্ড কালেকশন বাইরের কোড দ্বারা সরাসরি বিকৃত হওয়া রোধ করে অভ্যন্তরীণ স্টেট সুরক্ষিত রাখে।
  - ট্রানজ্যাকশনের সীমানা ছোট রেখে ডাটাবেস ডেডলক ও রেস কন্ডিশন প্রতিহত করে।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - একটি একক এসকিউএল ট্রানজ্যাকশনে একাধিক এগ্রিগেট রুট ক্যাসকেড সেভ করা নিষিদ্ধ।
  - ক্রস-এগ্রিগেট কাজের জন্য আউটবক্স প্যাটার্ন বা মেসেজ ব্রোকার প্রয়োজন হয়।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - `Microsoft.EntityFrameworkCore`: প্রাইভেট ব্যাকিং ফিল্ড ম্যাপ করার সুবিধা।
  - `Ardalis.GuardClauses`: ইনভ্যারিয়্যান্ট সুরক্ষায় গার্ড মেথড।"""))

BN_SECTIONS.append(("code", """// Production C# 12 / .NET Example: Value Objects, Strongly-Typed IDs & Aggregate Root
using System;
using System.Collections.Generic;

// ১. C# record struct দ্বারা তৈরি Strongly-Typed ID
public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
    public static CustomerId Empty => new(Guid.Empty);
}

// ২. Value Object: Money (ইমিউটেবল, সেলফ-ভ্যালিডেটেড, মুদ্রা হিসাব সম্বলিত)
public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "টাকার পরিমাণ ঋণাত্মক হতে পারবে না।");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException("মুদ্রা অবশ্যই ৩ অক্ষরের আইএসও কোড হতে হবে।", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "USD") => new(0m, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"ভিন্ন মুদ্রা {other.Currency} এর সাথে {Currency} যোগ করা সম্ভব নয়।");
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

// ৪. এগ্রিগেট রুট বেস ক্লাস (ইভেন্ট ট্র্যাকিং সংরক্ষণ করে)
public abstract class AggregateRoot<TId>
{
    public TId Id { get; protected set; } = default!;
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

// ৫. এগ্রিগেট রুট: Order (ইনভ্যারিয়্যান্ট ও ট্রানজ্যাকশন বাউন্ডারি পাহারা দেয়)
public sealed class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = new();

    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public bool IsCancelled { get; private set; }

    // EF Core এর জন্য প্রাইভেট কনস্ট্রাক্টর
    private Order() { }

    public Order(Guid id, CustomerId customerId, string currency)
    {
        Id = id;
        CustomerId = customerId;
        TotalAmount = Money.Zero(currency);
        IsCancelled = false;
    }

    // বিজনেস মেথড যা ইনভ্যারিয়্যান্ট নিশ্চিত করে
    public void AddOrderItem(Guid productId, int quantity, Money unitPrice)
    {
        if (IsCancelled)
            throw new InvalidOperationException("বাতিলকৃত অর্ডারে নতুন আইটেম যোগ করা যাবে না।");

        if (_items.Count >= 50)
            throw new InvalidOperationException("একটি অর্ডারে সর্বোচ্চ ৫০টির বেশি আইটেম থাকতে পারবে না।");

        _items.Add(new OrderItem(Guid.NewGuid(), productId, quantity, unitPrice));
        TotalAmount = TotalAmount.Add(new Money(unitPrice.Amount * quantity, unitPrice.Currency));

        // অন্য সিস্টেমগুলোকে অবহিত করার জন্য ডোমেন ইভেন্ট রেইজ করা
        RaiseDomainEvent(new OrderPlacedDomainEvent(Id, CustomerId, TotalAmount));
    }

    public void Cancel()
    {
        if (IsCancelled) return;
        IsCancelled = true;
    }
}

// চাইল্ড এনটিটি (এগ্রিগেট বাউন্ডারির বাইরে থেকে কখনো সরাসরি এক্সেস করা যাবে না)
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
}"""))

BN_SECTIONS.append(("markdown", """---

## ৬. আর্কিটেকচারাল কনসেপ্ট: ডোমেন ইভেন্টস এবং ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন (Domain Events & Transactional Outbox)

একটি **ডোমেন ইভেন্ট (Domain Event)** হলো অতীতে ঘটে যাওয়া এমন একটি তাৎপর্যপূর্ণ ব্যবসায়িক ঘটনা যা ডোমেন বিশেষজ্ঞদের কাছে মূল্যবান। এটি অপরিবর্তনীয় এবং অতীত কালের ক্রিয়ারূপে নামকরণ করা হয় (যেমন: `OrderPlacedEvent`, `PaymentFailedEvent`)।

![Aggregate Boundary & Outbox Lifecycle](images/aggregate-boundary-lifecycle.svg)

### ইন-প্রসেস ডোমেন ইভেন্ট বনাম ইন্টিগ্রেশন ইভেন্ট
- **ডোমেন ইভেন্টস**: বাউন্ডেড কনটেক্সটের অভ্যন্তরীণ ইভেন্ট। একই অ্যাপ্লিকেশনের ভেতরে MediatR বা সি-শার্প ডেলিগেটের মাধ্যমে সিঙ্ক্রোনাসলি প্রসেস করা যায়।
- **ইন্টিগ্রেশন ইভেন্টস**: নেটওয়ার্ক বাউন্ডারি অতিক্রম করে অন্যান্য মাইক্রোসার্ভিস বা বাউন্ডেড কনটেক্সটকে জানানোর জন্য মেসেজ ব্রোকারের (RabbitMQ, Kafka, Azure Service Bus) মাধ্যমে পাঠানো হয়।

### ডুয়াল-রাইট প্রবলেম এবং ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন
ডাটাবেসে এগ্রিগেট সেভ করা এবং ব্রোকারে ইভেন্ট পাঠানো—এই দুই কাজের মাঝে সিস্টেম ক্র্যাশ করলে ডেটা অসামঞ্জস্যতা (Dual-Write Failure) ঘটে। **ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন (Transactional Outbox Pattern)** এই সমস্যা সমাধান করে এগ্রিগেটের সাথে একই লোকাল ডাটাবেস ট্রানজ্যাকশনের মধ্যে একটি `OutboxMessages` টেবিলে ইভেন্টটি ইনসার্ট করার মাধ্যমে।

#### আর্কিটেকচারাল স্ট্যান্ডার্ড: ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন
- **সুফল (Benefits)**:
  - **গ্যারান্টিযুক্ত অ্যাট-লিস্ট-ওয়ান্স ডেলিভারি (At-least-once delivery)**: ব্রোকার সাময়িকভাবে ডাউন থাকলেও কোনো ইভেন্ট হারিয়ে যায় না।
  - **কোনো ডিস্ট্রিবিউটেড লক বা 2PC এর প্রয়োজন নেই**: ডাটাবেসের নেটিভ অ্যাসিড ট্রানজ্যাকশন ব্যবহার করে।
- **ড্রব্যাকস ও ট্রেড-অফ (Drawbacks & Trade-offs)**:
  - **ডুপ্লিকেট মেসেজ যাওয়ার সম্ভাবনা**: কনজিউমার সিস্টেমে অবশ্যই **আইডেমপোটেন্সি (Idempotency)** বা ইনবক্স প্যাটার্ন থাকতে হবে।
  - **ইভেনচুয়াল কনসিস্টেন্সি ল্যাগ**: ডাটাবেসে সেভ হওয়া এবং ব্রোকারে পাবলিশ হওয়ার মাঝে সামান্য সময়ের পার্থক্য তৈরি হয়।
- **.NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ**:
  - `MassTransit` ও `MassTransit.EntityFrameworkCoreIntegration`: প্রোডাকশন-রেডি বিল্ট-ইন আউটবক্স ও ইনবক্স প্যাটার্ন সাপোর্ট।
  - `MediatR`: ইন-প্রসেস ইভেন্ট নোটিফিকেশনের জন্য (`INotification`)।"""))

BN_SECTIONS.append(("code", """// Production C# 12 / .NET Example: EF Core Interceptor for Outbox Pattern & Domain Events
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

// ১. আউটবক্স টেবিল এনটিটি
public sealed class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}

// ২. EF Core SaveChanges ইন্টারসেপ্টর: স্বয়ংক্রিয়ভাবে ডোমেন ইভেন্টকে আউটবক্স মেসেজে রূপান্তর করে
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

        // চেঞ্জ ট্র্যাকারের মধ্য থেকে যেসব এগ্রিগেট ইভেন্ট রেইজ করেছে তাদের সংগ্রহ করা
        foreach (var entry in dbContext.ChangeTracker.Entries<AggregateRoot<Guid>>())
        {
            if (entry.Entity.DomainEvents.Count > 0)
            {
                domainEvents.AddRange(entry.Entity.DomainEvents);
                entry.Entity.ClearDomainEvents();
            }
        }

        // প্রতিটি ডোমেন ইভেন্টকে একই ডাটাবেস ট্রানজ্যাকশনে আউটবক্স রো হিসেবে সংরক্ষণ করা
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
}"""))

BN_SECTIONS.append(("markdown", """---

## ৭. আর্কিটেকচারাল কনসেপ্ট: ডোমেন সার্ভিসেস বনাম অ্যাপ্লিকেশন সার্ভিসেস (Domain vs. Application Services)

DDD-তে সবচেয়ে সাধারণ কনফিউশন হলো **ডোমেন সার্ভিস**, **অ্যাপ্লিকেশন সার্ভিস**, এবং **ইনফ্রাস্ট্রাকচার সার্ভিসের** মধ্যকার পার্থক্য নির্ণয় করা।

| বৈশিষ্ট্য (Attribute) | ডোমেন সার্ভিস (Domain Service) | অ্যাপ্লিকেশন সার্ভিস (Application Service) | ইনফ্রাস্ট্রাকচার সার্ভিস (Infrastructure Service) |
| :--- | :--- | :--- | :--- |
| **লেয়ার (Layer)** | ডোমেন লেয়ার (Core) | অ্যাপ্লিকেশন লেয়ার | ইনফ্রাস্ট্রাকচার লেয়ার |
| **মূল উদ্দেশ্য** | একাধিক এগ্রিগেটের মধ্যে সমন্বিত খাঁটি বিজনেস লজিক। | ইউজ কেস ও ট্রানজ্যাকশন অর্কেস্ট্রেশন। | I/O, নেটওয়ার্ক, ক্লাউড ও ফাইল সিস্টেম হ্যান্ডলিং। |
| **ডিপেন্ডেন্সি** | শুধুমাত্র ডোমেন অবজেক্ট (বাইরের ফ্রেমওয়ার্ক মুক্ত)। | রিপোজিটরি, ডোমেন সার্ভিস, ইভেন্ট বাস। | থার্ড-পার্টি SDK, EF Core, SMTP, AWS/Azure SDK। |
| **স্টেট (State)** | স্টেটলেস (Stateless)। | স্টেটলেস (Stateless)। | নেটওয়ার্ক কানেকশন পুল পরিচালনা করে। |
| **বাস্তব উদাহরণ** | `OrderPricingDomainService`, `TaxCalculator` | `CreateOrderCommandHandler`, `CheckoutUseCase` | `SmtpEmailSender`, `BlobStorageService` |

### কখন ডোমেন সার্ভিস তৈরি করবেন
একটি ডোমেন সার্ভিস তখনই তৈরি করা উচিত যখন কোনো বিজনেস অপারেশন:
1. অত্যন্ত গুরুত্বপূর্ণ বিজনেস ক্যালকুলেশন বা নিয়ম বহন করে।
2. একাধিক এগ্রিগেটের সাথে সম্পর্কিত যা কোনো একটি একক এগ্রিগেটের ভেতর জোর করে রাখলে দায়িত্বের নীতি (SRP) লঙ্ঘিত হয়।
3. এমন কোনো লজিক যা অ্যাপ্লিকেশন সার্ভিসে রাখা ঠিক নয় (কারণ অ্যাপ্লিকেশন সার্ভিসের ভেতর শূন্য বিজনেস রুলস থাকা উচিত)।

#### .NET / C# এর জন্য উপযুক্ত টেকনোলজি ও NuGet প্যাকেজ
- `MediatR`: অ্যাপ্লিকেশন লেয়ারের কমান্ড ও কুয়েরি হ্যান্ডলার তৈরি (`IRequestHandler<TCommand, TResult>`)।
- ডিপেন্ডেন্সি ইনজেকশন: ডোমেন সার্ভিসগুলোকে Scoped বা Transient হিসেবে রেজিস্টার করা।"""))

BN_SECTIONS.append(("code", """// Production C# 12 / .NET Example: Domain Service vs. Application Service
using System;
using System.Threading;
using System.Threading.Tasks;

// ১. ডোমেন সার্ভিস (একাধিক এগ্রিগেট বা বহিরাগত রেট সংক্রান্ত বিশুদ্ধ বিজনেস ক্যালকুলেশন)
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

// ২. অ্যাপ্লিকেশন সার্ভিস (শুধুমাত্র ব্যবহারের অর্কেস্ট্রেশন, কোনো বিজনেস রুলস থাকে না)
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
        // ১. এগ্রিগেট রুট তৈরি করা
        var order = new Order(Guid.NewGuid(), new CustomerId(request.CustomerId), request.Currency);

        // ২. রিপোজিটরির মাধ্যমে সেভ করা
        await _orderRepository.AddAsync(order, cancellationToken);

        // ৩. ইউনিট অব ওয়ার্ক কমিট করা (এগ্রিগেট ও আউটবক্স মেসেজ একই ট্রানজ্যাকশনে কমিট হয়)
        await _unitOfWork.CommitAsync(cancellationToken);

        return order.Id;
    }
}

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
}

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct);
}"""))

BN_SECTIONS.append(("markdown", """---

## ৮. আর্কিটেকচারাল সিনার্জি: CQRS, ইভেন্ট সোর্সিং এবং মাইক্রোসার্ভিসেস

### ৮.১ DDD + CQRS (Command Query Responsibility Segregation)
ঐতিহ্যবাহী আর্কিটেকচারে ডেটা পড়া (Read) এবং লেখার (Write) জন্য একই এনটিটি মডেল ব্যবহার করা হয়। জটিল সিস্টেমে এটি মারাত্মক সংঘাত তৈরি করে:
- **রাইট পাথ (Commands)**: ইনভ্যারিয়্যান্ট ভ্যালিডেশন নিশ্চিত করার জন্য সমস্ত চাইল্ড অবজেক্ট সহ এগ্রিগেট রুট মেমরিতে লোড করা বাধ্যতামূলক।
- **রিড পাথ (Queries)**: ইউজার ইন্টারফেসে দ্রুত ডেটা দেখানোর জন্য ফ্ল্যাট এবং জয়েন করা হালকা DTO দরকার।

```
        রাইট পাথ (DDD Aggregates)                       রিড পাথ (Fast Projections)
  ┌───────────────────────────────────┐           ┌────────────────────────────────────┐
  │ Command -> Handler -> Aggregate   │           │ Query -> Dapper / Fast SQL View    │
  │ -> Invariants -> EF Core Save     │           │ -> সরাসরি UI DTO রিটার্ন করে        │
  └───────────────────────────────────┘           └────────────────────────────────────┘
                    │                                               ▲
                    ▼                                               │
             [Write Database] ─── (CDC / Sync / Projection) ─── [Read Store]
```

DDD এর সাথে CQRS ব্যবহারের ফলে:
- **এগ্রিগেট কেবল রাইট পাথে (Write) ব্যবহৃত হয়**।
- **রিড পাথে এগ্রিগেট সম্পূর্ণ বাইপাস করা হয়**, এবং সরাসরি Dapper বা এসকিউএল ভিউ বা রিড রেপ্লিকা থেকে দ্রুত ডেটা রিড করা হয়।

---

### ৮.২ DDD + মাইক্রোসার্ভিসেস বনাম মডুলার মনোলিথ

অনেকেই ভুলবশত ধরে নেন যে **১টি বাউন্ডেড কনটেক্সট = ১টি মাইক্রোসার্ভিস**।

> [!WARNING]
> ডোমেনের সীমানা পরিপক্ক হওয়ার আগেই বাউন্ডেড কনটেক্সটগুলোকে আলাদা আলাদা মাইক্রোসার্ভিসে বিভক্ত করলে তা **ডিস্ট্রিবিউটেড মনোলিথ (Distributed Monolith)** তৈরি করে—যার ফলে মাইক্রোসার্ভিসের কোনো সুবিধাই পাওয়া যায় না, অথচ নেটওয়ার্ক ফেইলিউর ও ডিস্ট্রিবিউটেড ট্রানজ্যাকশনের সমস্ত জটিলতা ঘাড়ে এসে পড়ে।

- **সর্বোত্তম কৌশল (Martin Fowler's MonolithFirst)**:
  - প্রথমে একটি **মডুলার মনোলিথ (Modular Monolith)** প্রজেক্টে বাউন্ডেড কনটেক্সটগুলোকে আলাদা সি-শার্প প্রজেক্ট বা অ্যাসেম্বলি হিসেবে তৈরি করুন।
  - কনটেক্সটগুলোর মধ্যে যোগাযোগ ইন্টারনাল ইন্টারফেস ও MediatR এর মাধ্যমে ইন-প্রসেস রাখুন।
  - পরবর্তীতে যখন কোনো নির্দিষ্ট বাউন্ডেড কনটেক্সটের আলাদা স্কেলিং বা ভিন্ন টিমের স্বাধীন ডিপ্লয়মেন্ট দরকার হবে, তখনই কেবল সেটিকে আলাদা মাইক্রোসার্ভিসে রূপান্তরিত করুন।

---

## ৯. স্টাফ ও লিড লেভেল ইন্টারভিউ প্রশ্ন ও বিশদ উত্তর (Staff & Lead Interview Q&A)

### প্রশ্ন ১: ডোমেন-ড্রিভেন ডিজাইন (DDD) কী এবং এটি কখন সচেতনভাবে পরিহার করা উচিত?
**উত্তর**:
ডোমেন-ড্রিভেন ডিজাইন (DDD) হলো একটি সফটওয়্যার আর্কিটেকচার দর্শন যা কোডের ডিজাইনকে বাস্তব বিজনেস ডোমেনের গভীর মডেলের সাথে সরাসরি সংযুক্ত করে। এটি দুটি প্রধান অংশে বিভক্ত: **স্ট্র্যাটেজিক ডিজাইন** (ইউবিকুইটাস ল্যাঙ্গুয়েজ, সাবডোমেন, বাউন্ডেড কনটেক্সট, কনটেক্সট ম্যাপিং) এবং **ট্যাকটিক্যাল ডিজাইন** (এনটিটি, ভ্যালু অবজেক্ট, এগ্রিগেট, ডোমেন ইভেন্ট, রিপোজিটরি)।

**কখন DDD ব্যবহার পরিহার করবেন**:
1. **সাধারণ CRUD বা ডেটা-প্রধান সিস্টেম**: যেখানে বিজনেস রুলস অতি সাধারণ (যেমন: সাধারণ অ্যাডমিন প্যানেল বা সাধারণ রেকর্ড কিপিং)। এখানে এগ্রিগেট ও ভ্যালু অবজেক্ট বানালে অতিরিক্ত জটিলতা বাড়ে কিন্তু কোনো আর্থিক মূল্য পাওয়া যায় না।
2. **প্রযুক্তিগত ইউটিলিটি ও স্ট্রিমিং পাইপলাইন**: উচ্চ গতির টেলিমিতি ডাটা প্রসেসিং, লগ অ্যানালিটিক্স বা ETL পাইপলাইনে ডোমেন ইনভ্যারিয়্যান্টের চেয়ে Kafka Streams বা Spark-এর মতো ডেটা পাইপলাইন ফ্রেমওয়ার্ক বেশি জরুরি।
3. **প্রাথমিক আইডিয়া যাচাইয়ের প্রোটোটাইপ**: ২ সপ্তাহের হ্যাকাথনে যখন প্রোডাক্ট মার্কেট ফিট নিশ্চিত নয়, তখন ডোমেন মডেলিংয়ে সময় নষ্ট না করে Martin Fowler-এর Transaction Script বা Active Record প্যাটার্ন ব্যবহার করাই বুদ্ধিমানের কাজ।

---

### প্রশ্ন ২: এনটিটি (Entity) এবং ভ্যালু অবজেক্টের (Value Object) মূল পার্থক্য কী? একটি বাউন্ডেড কনটেক্সটের এনটিটি কি অন্য কনটেক্সটে ভ্যালু অবজেক্ট হতে পারে?
**উত্তর**:
- **এনটিটি (Entity)**: এর একটি সুনির্দিষ্ট আইডেন্টিটি (`Id`) থাকে যা তার পুরো লাইফসাইকেলে এক থাকে, এমনকি তার ভেতরের অন্যান্য বৈশিষ্ট্য বদলে গেলেও। এদের সমতা শুধুমাত্র আইডির ভিত্তিতে যাচাই করা হয় এবং এরা মিউটেবল (Mutable)।
- **ভ্যালু অবজেক্ট (Value Object)**: এর কোনো নিজস্ব আইডেন্টিটি থাকে না; এর সমস্ত প্রোপার্টির সমন্বয় দ্বারাই এটি সংজ্ঞায়িত হয়। দুটি ভ্যালু অবজেক্টের মান এক হলে তারা সম্পূর্ণ সমান। এরা কঠোরভাবে অপরিবর্তনীয় (Immutable); কোনো মান বদলাতে হলে সম্পূর্ণ নতুন একটি অবজেক্ট তৈরি করতে হয়।

**কনটেক্সট ভেদে রূপান্তর**:
**হ্যাঁ, অবশ্যই হতে পারে**। ডোমেনের বাউন্ডারির ওপর নির্ভর করে একই বাস্তব কনসেপ্ট ভিন্ন রূপ নিতে পারে:
- **শিপিং ও ডেলিভারি কনটেক্সটে**, `Address` একটি **এনটিটি** হতে পারে যার একটি `AddressId`, নির্দিষ্ট জিপিএস কো-অর্ডিনেট, ডেলিভারি হিস্ট্রি এবং রোড ট্রাফিক স্ট্যাটাস রয়েছে।
- কিন্তু **অর্ডারিং কনটেক্সটে**, `Address` কেবল একটি **ভ্যালু অবজেক্ট** (`Street`, `City`, `ZipCode`) যা চেকআউটের সময় অর্ডারের গায়ে স্ন্যাপশট হিসেবে বসে যায়। পরবর্তীতে গ্রাহক তার প্রোফাইল অ্যাড্রেস পরিবর্তন করলেও অতীতের অর্ডারে থাকা ভ্যালু অবজেক্টের ঠিকানায় কোনো পরিবর্তন আনা যাবে না।

---

### প্রশ্ন ৩: এগ্রিগেট রুট এবং "একটি ট্রানজ্যাকশনে কেবল একটি এগ্রিগেট পরিবর্তনের নিয়ম" ব্যাখ্যা করুন। বিজনেসে যদি একই সাথে দুটি এগ্রিগেট আপডেট করার প্রয়োজন পড়ে তবে কী করবেন?
**উত্তর**:
একটি **এগ্রিগেট** হলো ডাটা পরিবর্তনের কনসিস্টেন্সি ও ইনভ্যারিয়্যান্ট বাউন্ডারি। আর **এগ্রিগেট রুট** হলো সেই বাউন্ডারির প্রধান অভিভাবক এনটিটি যার মাধ্যমে বাইরের জগত ওই এগ্রিগেটের সাথে কথা বলে।

**১টি ট্রানজ্যাকশনে ১টি এগ্রিগেটের নিয়ম**:
একটি ডাটাবেস ট্রানজ্যাকশনে **শুধুমাত্র একটি এগ্রিগেট ইনস্ট্যান্স** পরিবর্তন ও কমিট করা উচিত। এর ফলে ডাটাবেস রো-লক কমে যায়, সিস্টেম ডেডলক মুক্ত থাকে এবং ডিস্ট্রিবিউটেড ডাটাবেসে সিস্টেম সহজে স্কেল করতে পারে।

**যদি দুটি এগ্রিগেট একই সাথে আপডেট করার রিকোয়ারমেন্ট আসে**:
1. **ইভেনচুয়াল কনসিস্টেন্সি গ্রহণ করুন (অনুমোদিত পদ্ধতি)**: এগ্রিগেট 'A' নিজের স্টেট আপডেট করে একটি ডোমেন ইভেন্ট রেইজ করে ট্রানজ্যাকশন কমিট করবে। একটি অ্যাসিনক্রোনাস কনজিউমার সেই ইভেন্ট পড়ে পরবর্তী ট্রানজ্যাকশনে এগ্রিগেট 'B' আপডেট করবে। কোনো কারণে এগ্রিগেট 'B' ফেইল করলে সাগা (Saga) বা ক্ষতিপূরণমূলক অ্যাকশন (Compensating Action) দিয়ে এগ্রিগেট 'A' রোলব্যাক/ক্যানসেল করা হবে।
2. **ইনভ্যারিয়্যান্ট বাউন্ডারি পুনর্মূল্যায়ন (Model Smell)**: যদি দুটি এনটিটি একই সাথে ইনস্ট্যান্টলি আপডেট না হলে চরম ব্যবসায়িক বিপর্যয় ঘটে, তবে সম্ভবত আপনার এগ্রিগেট বাউন্ডারি ভুল হয়েছে। তারা দুটি আলাদা এগ্রিগেট না হয়ে **একই এগ্রিগেটের অন্তর্ভুক্ত** হওয়া উচিত।

---

### প্রশ্ন ৪: অ্যানিমিক ডোমেন মডেল কেন একটি অ্যান্টি-প্যাটার্ন এবং C#/.NET-এ একে কীভাবে রিচ ডোমেন মডেলে রূপান্তর করবেন?
**উত্তর**:
**অ্যানিমিক ডোমেন মডেলে** ক্লাসগুলো কেবল প্রোপার্টি (Getter ও Setter) বহন করে এবং কোনো বিজনেস লজিক থাকে না। সমস্ত নিয়ম ছড়িয়ে থাকে বিভিন্ন সার্ভিস ক্লাসে।
- এটি অবজেক্ট-ওরিয়েন্টেড প্রোগ্রামিংয়ের মূল ভিত্তি এনক্যাপসুলেশন ভঙ্গ করে।
- যে কেউ যেকোনো জায়গা থেকে `order.Status = Shipped` লিখে ইনভ্যারিয়্যান্ট ভেঙে ফেলতে পারে।
- একই বিজনেস লজিক একাধিক কন্ট্রোলার ও সার্ভিসে ডুপ্লিকেট হয়।

**C#-এ রিচ ডোমেন মডেলে রূপান্তরের উপায়**:
1. সব প্রোপার্টির সেটারকে `private set` বা `private init` করুন।
2. পাবলিক প্যারামিটারবিহীন কনস্ট্রাক্টর বাদ দিয়ে প্যারামিটারাইজড কনস্ট্রাক্টর দিন যাতে বাধ্যতামূলক ফিল্ড ছাড়া অবজেক্ট তৈরিই না করা যায়।
3. প্রোপার্টিতে সরাসরি মান বসানোর বদলে অর্থপূর্ণ বিজনেস মেথড লিখুন (যেমন: `order.Cancel(string reason)` বা `order.AddLineItem(...)`)।
4. চাইল্ড কালেকশনগুলোকে প্রাইভেট ব্যাকিং ফিল্ডের পেছনে লুকিয়ে `IReadOnlyCollection<T>` আকারে এক্সপোজ করুন যাতে বাইরের কেউ সরাসরি `.Add()` বা `.Clear()` করতে না পারে।

---

### প্রশ্ন ৫: ডিস্ট্রিবিউটেড 2-Phase Commit (2PC) লক ছাড়া কীভাবে একাধিক এগ্রিগেটের মাঝে বিজনেস ইনভ্যারিয়্যান্ট রক্ষা করবেন?
**উত্তর**:
আধুনিক ক্লাউড মাইক্রোসার্ভিসে 2PC (যেমন MSDTC) মারাত্মক লেটেন্সি তৈরি করে এবং ডাটাবেস পারফরম্যান্স ধ্বংস করে দেয়। DDD-তে এটি সমাধান করা হয় নিম্নোক্ত উপায়ে:

1. **ডোমেন ইভেন্টস + সাগা প্যাটার্ন (Saga Pattern)**:
   - অর্ডার এগ্রিগেট তৈরি হয় `PendingPayment` স্টেটে এবং `OrderPlacedEvent` রেইজ করে।
   - পেমেন্ট সার্ভিস কার্ড চার্জ করার চেষ্টা করে। সফল হলে `PaymentCompletedEvent` আসে এবং অর্ডার `Paid` হয়; ব্যর্থ হলে `OrderCancelled` হয়।
2. **রিজার্ভেশন প্যাটার্ন (Reservation Pattern)**:
   - ইনভেন্টরি বা আসন সংরক্ষণে একটি নির্দিষ্ট সময়ের জন্য (যেমন ১০ মিনিট) রিসোর্স "Reserved" রাখা হয়। সময়ের মধ্যে পেমেন্ট না আসলে ব্যাকগ্রাউন্ড জবের মাধ্যমে তা স্বয়ংক্রিয়ভাবে খালাস হয়ে যায়।
3. **আইডেমপোটেন্ট কনজিউমার + ইনবক্স প্যাটার্ন**:
   - নেটওয়ার্ক ফেইলিউরের কারণে ইভেন্ট একাধিকবার আসলেও যেন ডাবল প্রসেসিং না হয়, সেজন্য প্রসেসড মেসেজ আইডি ট্র্যাক করা হয়।

---

### প্রশ্ন ৬: অ্যান্টি-করাপশন লেয়ার (ACL) কী? বাস্তব প্রোজেক্টে আপনি এটি কীভাবে বাস্তবায়ন করবেন?
**উত্তর**:
অ্যান্টি-করাপশন লেয়ার (ACL) হলো এমন একটি আইসোলেশন ও অনুবাদক স্তর যা একটি আধুনিক ক্লিন ডোমেন মডেল এবং একটি বাহ্যিক থার্ড-পার্টি বা পুরোনো লেগ্যাসি সিস্টেমের মাঝে প্রাচীর হিসেবে কাজ করে। এটি বাইরের সিস্টেমের অদ্ভুত মডেল ও পরিভাষাকে ভেতরের পরিচ্ছন্ন ইউবিকুইটাস ল্যাঙ্গুয়েজে রূপান্তর করে।

**বাস্তব প্রোজেক্টের উদাহরণ**:
- একটি আধুনিক ইকমার্স চেকআউট সিস্টেমের সাথে ২০ বছরের পুরোনো SAP ERP সিস্টেম ইন্টিগ্রেশন।
- SAP থেকে অদ্ভুত কোড আসত (`VBELN`, `MATNR`, `STAT = 'X'`) এবং SOAP এক্সএমএল দাবি করত।
- আমরা একটি ACL অ্যাডাপ্টার তৈরি করি:
  1. ডোমেনে একটি ক্লিন সি-শার্প ইন্টারফেস `IInventoryReservationService` তৈরি করা হয়।
  2. ACL অ্যাডাপ্টার ইন্টারনাল ক্লিন `ProductId` ও `Quantity` কে SAP SOAP পেলোডে রূপান্তর করে।
  3. Polly লাইব্রেরি দিয়ে রিট্রাই ও সার্কিট ব্রেকার যোগ করা হয়।
  4. পরবর্তীতে SAP পরিবর্তন করে আধুনিক Dynamics 365 বসানো হলেও ইন্টারনাল ডোমেনের ১ লাইন কোডও পরিবর্তন করতে হয়নি—শুধু ACL ইমপ্লিমেন্টেশন বদলানো হয়েছিল।

---

### প্রশ্ন ৭: গোস্ট ইভেন্ট (Ghost Events) বা ডুয়াল-রাইট ব্যর্থতা ছাড়াই কীভাবে নির্ভরযোগ্যভাবে ডোমেন ইভেন্ট পাবলিশ করবেন?
**উত্তর**:
এর সবচেয়ে নির্ভরযোগ্য ও স্ট্যান্ডার্ড সমাধান হলো **ট্রানজ্যাকশনাল আউটবক্স প্যাটার্ন (Transactional Outbox Pattern)**:
1. এগ্রিগেট রুটের মেথড কল হলে তা অভ্যন্তরীণ মেমরি লিস্টে ইভেন্ট জমিয়ে রাখে (`_domainEvents.Add(...)`)।
2. যখন `DbContext.SaveChangesAsync()` কল করা হয়, তখন একটি EF Core `SaveChangesInterceptor` স্বয়ংক্রিয়ভাবে সক্রিয় হয়।
3. ইন্টারসেপ্টরটি সমস্ত ডোমেন ইভেন্টকে JSON-এ সিরিয়ালাইজ করে একই লোকাল ডাটাবেস ট্রানজ্যাকশনের মধ্যে `OutboxMessages` টেবিলে ইনসার্ট করে দেয়।
4. ডাটাবেসে মূল এগ্রিগেট এবং আউটবক্স মেসেজ একটিমাত্র অ্যাসিড ট্রানজ্যাকশনে কমিট হয়; ফলে ডাটাবেস ক্র্যাশ করলে দুটোই একসাথে রোলব্যাক হয় (জিরো গোস্ট ইভেন্ট)।
5. পরবর্তীতে একটি ব্যাকগ্রাউন্ড সার্ভিস বা MassTransit আউটবক্স ওয়ার্কার আউটবক্স টেবিল থেকে মেসেজ পড়ে RabbitMQ বা Kafka তে পুশ করে এবং মেসেজটিকে `ProcessedOnUtc` হিসেবে মার্ক করে।

---

### প্রশ্ন ৮: এনক্যাপসুলেশন নষ্ট না করে Entity Framework Core দিয়ে কীভাবে জটিল DDD এগ্রিগেট ও ভ্যালু অবজেক্ট পারসিস্ট করবেন?
**উত্তর**:
আধুনিক EF Core (v7/8/9) ডোমেন এনক্যাপসুলেশনের সাথে চমৎকারভাবে সামঞ্জস্যপূর্ণ:
1. **প্রাইভেট ব্যাকিং ফিল্ড ম্যাপিং**:
   ```csharp
   builder.Property(x => x.TotalAmount)
          .HasField("_totalAmount")
          .UsePropertyAccessMode(PropertyAccessMode.Field);
   ```
2. **এনক্যাপসুলেটেড কালেকশন**:
   ```csharp
   builder.HasMany(x => x.Items)
          .WithOne()
          .HasForeignKey("OrderId");
   builder.Metadata.FindNavigation(nameof(Order.Items))!
          .SetPropertyAccessMode(PropertyAccessMode.Field);
   ```
3. **ভ্যালু অবজেক্টের জন্য `OwnsOne` বা `ComplexProperty`**:
   ```csharp
   builder.ComplexProperty(o => o.ShippingAddress, b => {
       b.Property(a => a.Street).HasColumnName("Street");
       b.Property(a => a.City).HasColumnName("City");
   });
   ```
4. **Strongly-Typed ID এর জন্য ভ্যালু কনভার্টার**:
   ```csharp
   builder.Property(o => o.Id)
          .HasConversion(id => id.Value, value => new OrderId(value));
   ```
5. **প্রাইভেট কনস্ট্রাক্টর**: EF Core রিফ্লেকশনের মাধ্যমে প্রাইভেট কনস্ট্রাক্টর দিয়ে অবজেক্ট তৈরি করতে পারে, ফলে কোনো পাবলিক প্যারামিটারহীন কনস্ট্রাক্টর রাখার প্রয়োজন নেই।

---

### প্রশ্ন ৯: ডোমেন সার্ভিস, অ্যাপ্লিকেশন সার্ভিস এবং ইনফ্রাস্ট্রাকচার সার্ভিসের সুস্পষ্ট সীমারেখা কী?
**উত্তর**:
- **ডোমেন সার্ভিস (Domain Service)**: খাঁটি ব্যবসায়িক নিয়ম ও গণনা ধারণ করে যা কোনো একটি একক এগ্রিগেটের মধ্যে রাখা যায় না। এর কোনো ডাটাবেস বা এক্সটার্নাল ফ্রেমওয়ার্ক ডিপেন্ডেন্সি থাকে না।
  - *উদাহরণ*: বিভিন্ন দেশের ট্যাক্স ও শুল্ক হিসাবকারী `TaxCalculationDomainService`।
- **অ্যাপ্লিকেশন সার্ভিস (Application Service)**: ইউজ কেসের চালক (Orchestrator)। এটি রিপোজিটরি থেকে এগ্রিগেট তোলে, ডোমেন মেথড কল করে, ইউনিট অব ওয়ার্ক কমিট করে এবং DTO ফেরত দেয়। এতে **শূন্য বিজনেস রুলস** থাকে।
  - *উদাহরণ*: `SubmitOrderCommandHandler` যা MediatR কমান্ড গ্রহণ করে পুরো প্রসেস পরিচালনা করে।
- **ইনফ্রাস্ট্রাকচার সার্ভিস (Infrastructure Service)**: প্রযুক্তিগত কাজ সম্পন্ন করে যেমন ইমেইল পাঠানো, ক্লাউড স্টোরেজে ফাইল রাখা বা থার্ড পার্টি এপিআই কল করা।
  - *উদাহরণ*: `SendGridEmailSender`, `AzureBlobStorageService`।

---

### প্রশ্ন ১০: হাই-কনটেনশন এগ্রিগেট (যেমন: ফ্ল্যাশ সেল ইনভেন্টরি, টিকিট বুকিং) DDD-তে কীভাবে হ্যান্ডেল করবেন?
**উত্তর**:
হাই-কনটেনশন এগ্রিগেটে হাজার হাজার রিকোয়েস্ট একই ডাটাবেস রো আপডেট করতে গিয়ে মারাত্মক অপটিমিস্টিক কনকারেন্সি এক্সেপশন (`DbUpdateConcurrencyException`) বা ডাটাবেস টাইমআউট তৈরি করে।

**আর্কিটেকচারাল সমাধান**:
1. **এগ্রিগেট বিভাজন (Decomposition)**:
   - একক `InventoryItem` এ ১০,০০০ স্টক না রেখে সেটিকে ১০০টি ছোট ছোট `StockBatch` এগ্রিগেটে (প্রতিটিতে ১০০টি করে স্টক) ভাগ করে দেওয়া যায় এবং ইউজার রিকোয়েস্টগুলোকে বিভিন্ন ব্যাচে র‍্যান্ডমলি রাউট করা যায়।
2. **ইনভার্টেড ফ্লো ও টোকেন বাকেট (Redis-first Flow)**:
   - ফ্ল্যাশ সেলের সময় সরাসরি ডাটাবেসে হিট না করে Redis-এর অ্যাটমিক স্ক্রিপ্ট (`DECRBY`) দিয়ে দ্রুত স্টক কমানো হয়।
   - ইউজারকে একটি ক্রিপ্টোগ্রাফিক `ReservationToken` দেওয়া হয়। ইউজার পরবর্তীতে শান্তভাবে অর্ডার প্লেস করে যা অ্যাসিনক্রোনাসলি ডাটাবেসে সেভ হয়।
3. **ইভেন্ট সোর্সিং (Event Sourcing)**:
   - রো আপডেট লকের বদলে শুধুমাত্র ইমিউটেবল `StockReservedEvent` ইনসার্ট করা হয়, যা এসকিউএল লকিং এড়িয়ে বিপুল রাইট থ্রুপুট প্রদান করে।

---

### প্রশ্ন ১১: প্রতিটি বাউন্ডেড কনটেক্সট কি বাধ্যতামূলকভাবে আলাদা মাইক্রোসার্ভিস হতে হবে? মডুলার মনোলিথের ভূমিকা কী?
**উত্তর**:
**কখনই না**। বাউন্ডেড কনটেক্সট হলো একটি *লজিক্যাল ডোমেন বাউন্ডারি*, আর মাইক্রোসার্ভিস হলো একটি *ফিজিক্যাল ডিপ্লয়মেন্ট বাউন্ডারি*।
- একটি একক কোডবেসের ভেতরে একাধিক বাউন্ডেড কনটেক্সটকে **মডুলার মনোলিথ (Modular Monolith)** আকারে অত্যন্ত সুশৃঙ্খলভাবে রাখা যায়।
- প্রতিটি বাউন্ডেড কনটেক্সট আলাদা আলাদা সি-শার্প প্রজেক্টে থাকবে এবং তাদের অভ্যন্তরীণ ক্লাসগুলো `internal` অ্যাক্সেস মডিফায়ারে সুরক্ষিত থাকবে।
- **মডুলার মনোলিথের সুবিধা**:
  - বাউন্ডারি পরিবর্তনের প্রয়োজন হলে সি-শার্প কোড রিফ্যাক্টরিং কয়েক মিনিটের ব্যাপার।
  - নেটওয়ার্কের লেটেন্সি ও নেটওয়ার্ক ফেইলিউর শূন্য।
  - জটিল কুবারনেটিস বা ডিস্ট্রিবিউটেড লগিংয়ের কোনো বাড়তি খরচ নেই।
- শুধুমাত্র তখনই একটি বাউন্ডেড কনটেক্সটকে আলাদা মাইক্রোসার্ভিসে রূপান্তর করা উচিত যখন তার জন্য আলাদা স্কেলিং রিকোয়ারমেন্ট বা সম্পূর্ণ আলাদা ইঞ্জিনিয়ারিং টিমের অটোনমি প্রয়োজন হয়।

---

### প্রশ্ন ১২: CQRS এবং ইভেন্ট সোর্সিং (Event Sourcing) ট্যাকটিক্যাল DDD-এর সাথে কীভাবে মিলে যায়? এর অপারেশনাল চ্যালেঞ্জগুলো কী?
**উত্তর**:
- **CQRS-এর মেলবন্ধন**: রাইট সাইডে ইনভ্যারিয়্যান্ট সুরক্ষার জন্য ভারী এগ্রিগেট ব্যবহার করা হয়, আর রিড সাইডে এগ্রিগেট বাইপাস করে দ্রুত পারফরম্যান্সের জন্য সরাসরি ফ্ল্যাট DTO বা এসকিউএল ভিউ রিড করা হয়।
- **ইভেন্ট সোর্সিং-এর মেলবন্ধন**: ডাটাবেসে এগ্রিগেটের বর্তমান স্টেট সেভ করার বদলে তার সমস্ত ডোমেন ইভেন্টের ক্রমানুসারে ধারাবাহিক স্ট্রিম সংরক্ষণ করা হয় (`OrderCreated`, `ItemAdded`)। এগ্রিগেটের বর্তমান অবস্থা জানতে ইভেন্টগুলো রিস্টোর বা রিপ্লে (`Apply(e)`) করা হয়।

**ইভেন্ট সোর্সিং-এর অপারেশনাল চ্যালেঞ্জসমূহ**:
1. **ইভেন্ট স্কিমা ভার্সনিং (Event Schema Evolution)**: বছরের পর বছর ধরে ব্যবসার নিয়মের সাথে ইভেন্টের স্কিমা বদলালে পুরোনো ইভেন্ট রূপান্তর করার জন্য জটিল Upcaster বা ভার্সনিং স্ট্র্যাটেজি লাগে।
2. **ইভেনচুয়াল কনসিস্টেন্সি ল্যাগ**: রিড মডেল প্রজেকশন আপডেট হতে কয়েক মিলিমিটার দেরি হলে ইউজার ফ্রন্টএন্ডে রিলোড দিয়ে পুরোনো ডেটা দেখতে পারেন।
3. **স্ন্যাপশট নেওয়ার জটিলতা**: হাজার হাজার ইভেন্ট থাকলে শূন্য থেকে এগ্রিগেট রিবিল্ড করতে দেরি হয়; তাই নিয়মিত ইন্টারভালে স্ন্যাপশট স্টেট সেভ করতে হয়।
4. **GDPR ও ব্যক্তিগত তথ্য মোছার অধিকার (Right to be Forgotten)**: ইমিউটেবল অ্যাপেন্ড-অনলি ইভেন্ট স্টোর থেকে গ্রাহকের নাম-ঠিকানা ডিলিট করা কঠিন; এর জন্য ক্রিপ্টোগ্রাফিক ইরেজার (এনক্রিপশন কি মুছে ফেলা) ব্যবহার করতে হয়।"""))

print("Bangla content loaded successfully.")

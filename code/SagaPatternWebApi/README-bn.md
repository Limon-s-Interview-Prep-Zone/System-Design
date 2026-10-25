# MassTransit, RabbitMQ এবং SQLite সহ ই-কমার্স সাগা অর্কেস্ট্রেশন (Saga Orchestration)

**ASP.NET Core 8**, **MassTransit 8.3 (Automatonymous State Machine)**, **RabbitMQ মেসেজ ব্রোকার**, **SQLite (EF Core)**, এবং **OpenTelemetry (ডিস্ট্রিবিউটেড ট্রেসিং ও মেট্রিক্স)** ব্যবহার করে নির্মিত একটি প্রোডাকশন-গ্রেড **সাগা প্যাটার্ন (Saga Pattern - Orchestration)** ইমপ্লিমেন্টেশন।

---

## অবজারভেবিলিটি এবং ওপেনটেলিমেট্রি (OpenTelemetry Distributed Tracing)

প্রতিটি মেসেজ এবং সাগা স্টেট মেশিন ট্রানজিশন স্বয়ংক্রিয়ভাবে একটি ইউনিফাইড W3C `TraceId` ডিস্ট্রিবিউটেড ট্রেসে যুক্ত হয়:

```
[HTTP POST /api/orders/checkout] (TraceId: 74a498c6154999d373f8e24554e4473a)
    │
    ├──► [MassTransit: SubmitOrderCommand send]
    │       │
    │       ▼ (RabbitMQ traceparent envelope)
    ├──► [Consumer: OrderCommand process]
    │       │
    │       ├──► [EF Core SQLite: INSERT INTO Orders]
    │       └──► [MassTransit: OrderCreatedEvent send]
    │               │
    │               ▼
    ├──► [Saga: OrderState process (Initially -> Submitted)]
    │       └──► [MassTransit: ReserveInventoryCommand send]
    │               │
    │               ▼
    ├──► [Consumer: InventoryCommand process]
    │       └──► [MassTransit: InventoryReservedEvent send]
    │               │
    │               ▼
    ├──► [Consumer: PaymentCommand process]
    │       └──► [MassTransit: PaymentProcessedEvent send]
    │               │
    │               ▼
    └──► [Consumer: OrderCommand complete] (Status = Completed)
```

---

## আর্কিটেকচার এবং ফ্লোসমূহ (Architecture & Flows)

### ১. ফরোয়ার্ড ফ্লো / হ্যাপি পাথ (Forward Flow - Happy Path)
```
[POST /api/orders/checkout]
       │
       ▼ (SubmitOrderCommand)
[OrderCommandConsumer] ──> অর্ডার সেভ (Status = Created)
       │
       ▼ (OrderCreatedEvent)
[OrderStateMachine] ────> সাগা স্টেট মেশিন শুরু (State = Submitted)
       │
       ▼ (ReserveInventoryCommand)
[InventoryCommandConsumer] ──> ইনভেন্টরি স্টক রিজার্ভ (সফল)
       │
       ▼ (InventoryReservedEvent)
[OrderStateMachine] ────> স্টেট পরিবর্তন (State = AwaitingPayment)
       │
       ▼ (ProcessPaymentCommand)
[PaymentCommandConsumer] ───> পেমেন্ট চার্জ সম্পন্ন (সফল)
       │
       ▼ (PaymentProcessedEvent)
[OrderStateMachine] ────> স্টেট পরিবর্তন (State = Completed)
       │
       ▼ (CompleteOrderCommand)
[OrderCommandConsumer] ──> অর্ডার সম্পন্ন হিসেবে আপডেট (Status = Completed)
```

### ২. ফেইলিউর এবং ক্ষতিপূরণ ফ্লো (Failure & Compensation Flow - Payment Declined)
```
[POST /api/orders/checkout/fail-payment] ($1500 > $1000 limit)
       │
       ▼ (SubmitOrderCommand)
[OrderCommandConsumer] ──> অর্ডার তৈরি (Status = Created)
       │
       ▼ (OrderCreatedEvent)
[OrderStateMachine] ────> স্টেট পরিবর্তন (State = Submitted)
       │
       ▼ (ReserveInventoryCommand)
[InventoryCommandConsumer] ──> ইনভেন্টরি স্টক রিজার্ভ (সফল)
       │
       ▼ (InventoryReservedEvent)
[OrderStateMachine] ────> স্টেট পরিবর্তন (State = AwaitingPayment)
       │
       ▼ (ProcessPaymentCommand)
[PaymentCommandConsumer] ───> পেমেন্ট ব্যর্থ! ($1500 লিমিট $1000 এর বেশি)
       │
       ▼ (PaymentFailedEvent)
[OrderStateMachine] ────> ত্রুটি শনাক্ত! স্টেট পরিবর্তন (State = Cancelled)
       ├──► (ReleaseInventoryCommand) ──> [InventoryCommandConsumer] (পূর্বে রিজার্ভ করা স্টক রিলিজ/ক্ষতিপূরণ)
       └──► (CancelOrderCommand)      ──> [OrderCommandConsumer] (অর্ডার স্ট্যাটাস = Cancelled করা)
```

---

## প্রজেক্ট ডিরেক্টরি কাঠামো (Project Structure)

```
SagaPatternWebApi/
├── Contracts/
│   ├── Commands.cs       # SubmitOrder, ReserveInventory, ProcessPayment, ReleaseInventory, CancelOrder, CompleteOrder
│   └── Events.cs         # OrderCreated, InventoryReserved, PaymentProcessed, PaymentFailed, ইত্যাদি
├── Consumers/
│   ├── OrderCommandConsumer.cs      # Submit, Complete, এবং Cancel Order কমান্ড প্রসেসিং
│   ├── InventoryCommandConsumer.cs  # Reserve এবং ক্ষতিপূরণমূলক Release Inventory কমান্ড প্রসেসিং
│   └── PaymentCommandConsumer.cs    # Process Payment কমান্ড প্রসেসিং
├── Data/
│   ├── Order.cs          # বিজনেস ডোমেন এন্টিটি (Business Domain Entity)
│   ├── OrderState.cs     # সাগা স্টেট মেশিন ইনস্ট্যান্স এন্টিটি (CorrelationId, CurrentState, Version)
│   └── AppDbContext.cs   # EF Core DbContext (WAL মোড ও Outbox সাপোর্ট সহ)
├── StateMachines/
│   └── OrderStateMachine.cs  # MassTransit Automatonymous স্টেট মেশিন কনফিগারেশন
├── Controllers/
│   └── OrdersController.cs   # ফ্লো ট্রিগার এবং ট্র্যাক করার REST API এন্ডপয়েন্ট
├── Program.cs            # ডিপেন্ডেন্সি ইনজেকশন, MassTransit, RabbitMQ ও SQLite সেটআপ
└── test_flows.ps1        # স্বয়ংক্রিয় টেস্টিং স্ক্রিপ্ট (সব ৩টি ফ্লো ভেরিফিকেশন)
```

---

## প্রয়োজনীয় সফটওয়্যার (Prerequisites)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- RabbitMQ লোকাল পোর্টে রানিং থাকতে হবে (`localhost:5672`, ডিফল্ট ক্রেডেনশিয়াল: `guest` / `guest`)

---

## অ্যাপ্লিকেশন চালানোর নিয়ম (Running the Application)

১. **API শুরু করুন:**
   ```powershell
   cd D:\Interview\System-Design\SagaPatternWebApi
   dotnet run
   ```

২. **Swagger UI ওপেন করুন:**
   ব্রাউজারে যান: [http://localhost:5150](http://localhost:5150)

---

## টেস্টিং নির্দেশিকা (Testing Scenarios)

### পরিস্থিতি ক: ফরোয়ার্ড ফ্লো / সফল অর্ডার (Happy Path)
```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5150/api/orders/checkout" `
  -ContentType "application/json" `
  -Body '{"customerId":"alice","amount":150.00,"quantity":2}'
```
অর্ডার এবং সাগা স্ট্যাটাস যাচাই করুন:
```powershell
Invoke-RestMethod -Uri "http://localhost:5150/api/orders/<OrderId>"
Invoke-RestMethod -Uri "http://localhost:5150/api/orders/<OrderId>/saga-state"
```
**প্রত্যাশিত ফলাফল:**
- Order Status: `Completed`
- Saga CurrentState: `Completed`

---

### পরিস্থিতি খ: ক্ষতিপূরণ ফ্লো / পেমেন্ট ব্যর্থ (Payment Declined & Compensation)
```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5150/api/orders/checkout/fail-payment?customerId=bob&amount=1500"
```
**প্রত্যাশিত ফলাফল:**
- অর্ডার অ্যামাউন্ট ($1500) সর্বোচ্চ লিমিট ($1000) অতিক্রম করায় পেমেন্ট ব্যর্থ হয়।
- সাগা স্টেট মেশিন স্বয়ংক্রিয়ভাবে ক্ষতিপূরণমূলক `ReleaseInventoryCommand` ইস্যু করে ইনভেন্টরি রিলিজ করে।
- Order Status: `Cancelled`
- Saga CurrentState: `Cancelled`
- Reason: `Card declined: Amount exceeds per-transaction limit ($1,000.00)`

---

### পরিস্থিতি গ: স্বয়ংক্রিয় টেস্ট স্ক্রিপ্ট (Automated Test Runner)
```powershell
cd D:\Interview\System-Design\SagaPatternWebApi
powershell -ExecutionPolicy Bypass -File .\test_flows.ps1
```

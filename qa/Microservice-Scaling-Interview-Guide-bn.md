# সিনিয়র সিস্টেম ডিজাইন ইন্টারভিউ: মাইক্রোসার্ভিস স্কেলিং (Scaling a Microservice)

---

## ১. ওভারভিউ ও ইন্টারভিউ প্রম্পট

**প্রম্পট**: *একটি মাইক্রোসার্ভিস আর্কিটেকচার ডিজাইন করুন যা ট্রাফিকের দ্রুত বৃদ্ধি সামলাতে পারে, ল্যাটেন্সি কম রাখে, সাশ্রয়ী হয় এবং উচ্চ প্রাপ্যতা (High Availability) নিশ্চিত করে।*

ইন্টারভিউতে **আর্কিটেকচারাল বিবর্তন**, **ক্যাপাসিটি এস্টিমেশন**, এবং সুনির্দিষ্ট **.NET/C# প্যাটার্ন** ব্যাখ্যা করতে হবে।

---

## ২. স্টেজ ১ – ক্ল্যারিফাইং প্রশ্ন এবং স্কোপ (Clarifying Questions & Scope)

| ক্যাটাগরি | সাধারণ প্রশ্নসমূহ |
|----------|-------------------|
| ফাংশনাল | মূল এপিআইগুলো কী কী? দীর্ঘ সময় ধরে চলা কোনো অপারেশন (ফাইল প্রসেসিং, রিপোর্ট জেনারেশন) আছে কি? |
| নন-ফাংশনাল | টার্গেট **QPS**, ল্যাটেন্সি SLA (p99), প্রাপ্যতা (%), কনসিস্টেন্সি মডেল, আনুমানিক ডেটা ভলিউম, ভৌগোলিক বিস্তৃতি |
| ডিপ্লয়মেন্ট | পছন্দের ক্লাউড প্রোভাইডার? বিদ্যমান CI/CD পাইপলাইন? নির্দিষ্ট ফ্রেমওয়ার্কের কোনো বাধ্যবাধকতা আছে কি? |
| অপারেশনাল | কাঙ্ক্ষিত অবজারভেবিলিটি (মেট্রিক্স, ট্রেসিং), রেট-লিমিটিং পলিসি, ফেইল-ওভার প্রয়োজনীয়তা |

এই উত্তরগুলো পরবর্তী প্রতিটি ডিজাইন সিদ্ধান্তকে পরিচালিত করে।

---

## ৩. স্টেজ ২ – ব্যাক-অফ-দ্য-এনভেলপ ক্যাপাসিটি এস্টিমেশন

> **ধরে নেওয়া যাক**: ৫০,০০০ সমসাময়িক সক্রিয় ব্যবহারকারী (Concurrent Active Users), ব্যবহারকারী প্রতি মিনিটে গড়ে ১০টি রিকোয়েস্ট $\rightarrow$ ৮,৩০০ QPS (গড়) এবং ৫ গুণ পিক ফ্যাক্টর $\rightarrow$ **≈ ৪২,০০০ পিক QPS**।

| মেট্রিক | হিসাব |
|--------|-------|
| **গড় QPS (Average QPS)** | `(৫০,০০০ ইউজার * ১০ রিকোয়েস্ট/মিনিট) / ৬০ ≈ ৮,৩০০ QPS` |
| **পিক QPS (Peak QPS)** | `৮,৩০০ * ৫ ≈ ৪২,০০০ QPS` |
| **রিড/রাইট অনুপাত** | ৮৫% রিড, ১৫% রাইট $\rightarrow$ ৩৫,৭০০ রিড QPS / ৬,৩০০ রাইট QPS |
| **প্রতিদিনের রাইট ভলিউম** | `৬,৩০০ QPS * ৮৬,৪০০ সেকেন্ড * ১ KB ≈ ৫৪০ GB/দিন` |
| **ক্যাশ র‍্যাম (৮০/২০ নিয়ম)** | `রিড ডেটা ভলিউম * ০.২০` $\rightarrow$ ২ KB পেলোড ধরে $\rightarrow$ `৩৫.৭k * ২ KB * ৮৬,৪০০ * ০.২০ ≈ ১২৩ GB RAM` |

এই সংখ্যাগুলো অটো-স্কেলিং থ্রেশহোল্ড, ক্যাশ সাইজিং এবং নেটওয়ার্ক প্রভিশনিং নির্ধারণ করে।

---

## ৪. স্টেজ ৩ – হাই-লেভেল আর্কিটেকচার ডায়াগ্রাম

![Microservice Scaling Architecture](../images/microservice-scaling-architecture.svg)

ডায়াগ্রামটি একক নোড থেকে শুরু করে সম্পূর্ণ ডিস্ট্রিবিউটেড এবং রেজিলিয়েন্ট মাইক্রোসার্ভিস বিবর্তন প্রদর্শন করে।

---

## ৫. স্টেজ ৪ – ডিপ ডাইভ – স্কেলিং লেভারসমূহ (ধাপে ধাপে)

### ৫.১ সার্ভিসটিকে স্টেটলেস (Stateless) করা
- **কেন**: স্টিকি সেশন (Sticky Sessions) ছাড়াই হরাইজন্টাল পড অটো-স্কেলিং নিশ্চিত করে।
- **করণীয়**: সেশন স্টেট, ইন-প্রসেস ক্যাশ এবং অস্থায়ী ফাইল বাহ্যিক স্টোরেজে স্থানান্তর করা।

```csharp
// সেশন স্টেট রেডিজে (Redis) এক্সটার্নালাইজ করা
builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = "redis:6379";
    o.InstanceName = "SessionStore:";
});
builder.Services.AddSession(o => o.IdleTimeout = TimeSpan.FromMinutes(30));
```

### ৫.২ কনটেইনারাইজেশন এবং অর্কেস্ট্রেটর ডিপ্লয়মেন্ট
- একটি লাইটওয়েট মাল্টি-স্টেজ ডকার ইমেজ তৈরি করুন।
- কুবারনেটিসে (Kubernetes) **Deployment** এবং **Service** এর মাধ্যমে ডিপ্লয় করুন।

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myservice
spec:
  replicas: 3
  selector:
    matchLabels:
      app: myservice
  template:
    metadata:
      labels:
        app: myservice
    spec:
      containers:
      - name: myservice
        image: myregistry/myservice:latest
        ports:
        - containerPort: 80
        resources:
          limits:
            cpu: "500m"
            memory: "256Mi"
```

### ৫.৩ অটো-স্কেলিং – হরাইজন্টাল পড অটো-স্কেলার (HPA)
- **মেট্রিক্স**: CPU $\le 70\%$ অথবা প্রমিথিউস থেকে কাস্টম QPS মেট্রিক।

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: myservice-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: myservice
  minReplicas: 2
  maxReplicas: 20
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Pods
    pods:
      metric:
        name: http_requests_per_second
      target:
        type: AverageValue
        averageValue: 1000
```

### ৫.৪ মাল্টি-লেভেল ক্যাশিং (L0-L2)
| লেয়ার | উদ্দেশ্য | প্রযুক্তি |
|------|---------|----------|
| **L0 – CDN** | স্ট্যাটিক অ্যাসেট, ইমেজ, সিএসএস, জেএস | Cloudflare / Azure Front Door |
| **L1 – ইন-প্রসেস (`MemoryCache`)** | হট-কী রিড, জিরো নেটওয়ার্ক ল্যাটেন্সি | `builder.Services.AddMemoryCache();` |
| **L2 – ডিস্ট্রিবিউটেড (`Redis`)** | পড জুড়ে শেয়ার্ড ক্যাশ, রিস্টার্টেও অক্ষত থাকে | `builder.Services.AddStackExchangeRedisCache(...);` |

*ক্যাশ-অ্যাসাইড প্যাটার্ন (C#):*

```csharp
public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl)
{
    var cached = await _redisCache.GetStringAsync(key);
    if (cached != null) return JsonSerializer.Deserialize<T>(cached);

    var result = await factory();
    await _redisCache.SetStringAsync(key, JsonSerializer.Serialize(result), 
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl });
    return result;
}
```

### ৫.৫ অ্যাসিঙ্ক্রোনাস ইভেন্ট-ড্রিভেন ডিকপলিং
- দীর্ঘমেয়াদী কাজগুলো (ইমেইল, পিডিএফ, অ্যানালিটিক্স) মেসেজ ব্রোকারে পাঠিয়ে দিন।
- এক্সাক্টলি-ওয়ান্স (Exactly-Once) নির্ভরযোগ্যতার জন্য **Transactional Outbox** ব্যবহার করুন।

```csharp
// MassTransit + Outbox (Postgres)
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
        o.QueryDelay = TimeSpan.FromSeconds(1);
    });
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.ConfigureEndpoints(ctx);
    });
});
```

### ৫.৬ ডেটাবেস স্কেলিং ফানেল
1. **রিড-রাইট স্প্লিটিং** – রাইটের জন্য প্রাইমারি মাস্টার, সিলেক্ট কোয়েরির জন্য রিড রেপ্লিকা।
2. **কানেকশন পুলিং** – PgBouncer অথবা Npgsql পুলিং।
3. **শার্ডিং** – `UserId` বা টেন্যান্ট আইডির ওপর কনসিস্টেন্ট হ্যাশিং।
4. **CQRS** – রিড ও রাইট মডেল আলাদা করা।

```csharp
// ডেটাবেস কলের জন্য Polly সার্কিট ব্রেকার
var circuitBreaker = Policy
    .Handle<Exception>()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

await circuitBreaker.ExecuteAsync(async () =>
{
    await dbContext.SaveChangesAsync();
});
```

### ৫.৭ অবজারভেবিলিটি এবং রেট লিমিটিং
- **মেট্রিক্স**: Prometheus এক্সপোর্টার (`prometheus-net`)।
- **ট্রেসিং**: OpenTelemetry $\rightarrow$ Jaeger।
- **রেট লিমিটিং**: টোকেন বাকেট অ্যালগরিদম (`Microsoft.AspNetCore.RateLimiting`)।

```csharp
app.UseRateLimiter(new RateLimiterOptions
{
    GlobalLimiter = PartitionedRateLimiter.CreateTokenBucketLimiter(
        partition => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 1000,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 100,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = 1000,
            AutoReplenishment = true
        })
});
```

### ৫.৮ সার্ভিস মেশ (Service Mesh - বড় ডিপ্লয়মেন্টের জন্য)
- সেন্ট্রালাইজড ট্রাফিক ম্যানেজমেন্ট, রিট্রাই, mTLS এবং টেলিমেট্রির জন্য **Istio** বা **Linkerd** ডিপ্লয় করুন।

---

## ৬. স্টেজ ৫ – ইন্টারভিউ সারাংশ কার্ড

| স্কেলিং কৌশল | সাধারণ ভুল ধারণা | প্রোডাকশন গ্রেড সমাধান |
|--------------|------------------|------------------------|
| **স্টেটলেসনেস** | মেমোরিতে সেশন রাখা | এক্সটার্নাল Redis / JWT (কোনো অ্যাফিনিটি নেই) |
| **ভার্টিক্যাল স্কেলিং** | বড় সাইজের VM কেনা | হরাইজন্টাল পড + কুবারনেটিস HPA |
| **ক্যাশিং** | কোনো ক্যাশ না রাখা | L0 CDN, L1 MemoryCache, L2 Redis ক্লাস্টার |
| **সিঙ্ক প্রসেসিং** | রিকোয়েস্ট থ্রেডে সব কাজ করা | অ্যাসিঙ্ক ইভেন্ট কিউ + আউটবক্স প্যাটার্ন |
| **ডিবি থ্রুপুট** | একক মাস্টার ডেটাবেস | রিড রেপ্লিকা $\rightarrow$ শার্ডিং $\rightarrow$ CQRS |
| **রেজিলিয়েন্স** | নিয়ন্ত্রণহীন রিট্রাই | Polly সার্কিট ব্রেকার + এক্সপোনেনশিয়াল ব্যাকঅফ |
| **অবজারভেবিলিটি** | বিচ্ছিন্ন লগ ফাইল | Prometheus + OpenTelemetry + টোকেন বাকেট রেট লিমিটার |

---

## ৭. ইন্টারভিউ কুইক চেকলিস্ট
1. **Stateless** – সেশন এক্সটার্নালাইজ করুন, লোকাল ডিস্কে ফাইল রাখা বন্ধ করুন।  
2. **Container + Orchestrator** – ডকার ও কুবারনেটিস ডিপ্লয়মেন্ট।  
3. **Autoscale** – CPU এবং কাস্টম QPS মেট্রিকের ওপর HPA।  
4. **Cache Hierarchy** – CDN $\rightarrow$ MemoryCache $\rightarrow$ Redis।  
5. **Async Decoupling** – MassTransit + Outbox প্যাটার্ন।  
6. **DB Funnel** – রেপ্লিকা $\rightarrow$ শার্ডিং $\rightarrow$ CQRS।  
7. **Resilience** – Polly সার্কিট ব্রেকার, টোকেন বাকেট রেট লিমিটিং।  
8. **Observability** – Prometheus + OpenTelemetry।  
9. **Service Mesh** – গ্লোবাল ট্রাফিক কন্ট্রোলের জন্য Istio।


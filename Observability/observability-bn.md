# ডিস্ট্রিবিউটেড সিস্টেমে অবজারভেবিলিটি এবং টেলিমেট্রি ইঞ্জিনিয়ারিং (Observability & Telemetry)

মনোলিথিক অ্যাপ্লিকেশনে কোনো ত্রুটি নির্ণয় করতে কেবল একটি লোকাল লগ ফাইল পড়া বা ডিবাগার সংযুক্ত করাই যথেষ্ট। কিন্তু আধুনিক ডিস্ট্রিবিউটেড মাইক্রোসার্ভিস আর্কিটেকচারে—যেখানে একজন ইউজারের একটি মাত্র ক্লিকের ফলে ক্লাউড কন্টেইনার জুড়ে ডজন ডজন অ্যাসিনক্রোনাস ইভেন্ট, রিমোট প্রসিডিউর কল (gRPC/REST) এবং ডাটাবেজ ট্রানজ্যাকশন সম্পন্ন হয়—সেখানে সনাতন মনিটরিং ব্যবস্থা পুরোপুরি অকার্যকর হয়ে পড়ে।

**অবজারভেবিলিটি (Observability)** হলো একটি ডিস্ট্রিবিউটেড সিস্টেমের এমন একটি বৈশিষ্ট্য যার মাধ্যমে সিস্টেমের অভ্যন্তরীণ জটিল অবস্থা কেমন আছে, তা শুধুমাত্র তার বাহ্যিক আউটপুট বা টেলিমেট্রি ডাটা বিশ্লেষণ করে নিখুঁতভাবে অনুমান ও অনুধাবন করা যায়।

![The 3 (+1) Pillars of Observability](images/observability-pillars-overview.svg)

---

## ১. অবজারভেবিলিটির ভিত্তি এবং ৩ (+১) স্তম্ভ (M.E.L.T. + Profiling)

### মনিটরিং বনাম অবজারভেবিলিটি: মূল পার্থক্য
- **মনিটরিং (Monitoring)**: উত্তর দেয় *"সিস্টেমটি কি কাজ করছে?"*—এটি পূর্বনির্ধারিত থ্রেশহোল্ড পর্যবেক্ষণ করে (যেমন: CPU > ৮০%, ডিস্ক স্পেস, পিং রেসপন্স)। এটি পূর্বপরিচিত ত্রুটিসমূহ রিপোর্ট করে (**Known-Unknowns**)।
- **অবজারভেবিলিটি (Observability)**: উত্তর দেয় *"সিস্টেমটি কেন এমন আচরণ করছে?"*—এটি জটিল ও অপ্রত্যাশিত পরিবেশের অভ্যন্তরীণ ত্রুটি নির্ণয়ের জন্য গভীর প্রাসঙ্গিক টেলিমেট্রি বিশ্লেষণ করে (**Unknown-Unknowns**)।

### ৪টি মৌলিক টেলিমেট্রি প্রকারভেদ

| টেলিমেট্রি স্তম্ভ | মূল উপাদান (Primitive) | কোন মূল প্রশ্নের উত্তর দেয়? | ডাটা ভলিউম ও খরচ | সাধারণ টেকনোলজি স্ট্যাক |
| :--- | :--- | :--- | :--- | :--- |
| **১. মেট্রিক্স (Metrics)** | টাইম-সিরিজ সমষ্টি (Counters, Gauges, Histograms) | *"কোথায় কী সমস্যা হয়েছে এবং কখন হয়েছে?"* | **কম** (সংখ্যাতাত্ত্বিক এগ্রিগেশন; মেমোরি সাইজ স্থির থাকে) | Prometheus, Thanos, AWS CloudWatch, Datadog |
| **২. লগস (Logs)** | টাইমস্ট্যাম্পযুক্ত স্ট্রাকচার্ড JSON ইভেন্ট রেকর্ড | *"ত্রুটিটি কেন ঘটেছে (রুট কজ কী)?"* | **উচ্চ** (ট্রাফিকের সাথে সরাসরি বৃদ্ধি পায়; ইন্ডেক্সিং ব্যয়বহুল) | Grafana Loki, Elasticsearch, OpenSearch, Fluentd |
| **৩. ট্রেসেস (Traces)** | নেটওয়ার্ক হপ জুড়ে স্প্যানসমূহের নির্দেশিত গ্রাফ (DAG) | *"সিস্টেমের কোন সার্ভিসে ঠিক কতটা বিলম্ব (ল্যাটেন্সি) হয়েছে?"* | **মাঝারি - উচ্চ** (ইন্টেলিজেন্ট স্যাম্পলিং প্রয়োজন) | Grafana Tempo, Jaeger, Zipkin, AWS X-Ray |
| **৪. কন্টিনিউয়াস প্রোফাইলিং** | লাইভ রানটাইম স্ট্যাক ট্রেস এবং ফ্লেম গ্রাফ (Flame Graphs) | *"কোডের ঠিক কোন মেথডটি সবচেয়ে বেশি সিপিইউ বা মেমোরি পোড়াচ্ছে?"* | **কম** (কার্নেল লেভেল পরিসংখ্যানিক স্যাম্পলিং) | Grafana Pyroscope, Parca, Datadog Profiler, dotnet-trace |

---

### ক. মেট্রিক্সের ৪টি প্রাথমিক ধরন
1. **Counter**: একটি ক্রমবর্ধনশীল মান যা কেবল বাড়ে অথবা সিস্টেম রিবুট হলে শূন্য হয় (যেমন: `http_requests_total`)। এর পরিবর্তনের হার পরিমাপ করা হয় $\text{rate}()$ ফাংশন দিয়ে:
   $$\text{Requests Per Second (RPS)} = \text{rate}(\text{http\_requests\_total}[5m])$$
2. **Gauge**: যেকোনো মুহূর্তে হ্রাস-বৃদ্ধি হতে পারে এমন মান (যেমন: `active_connections`, `memory_usage_bytes`, `threadpool_queue_length`)।
3. **Histogram**: নির্দিষ্ট সময়সীমার মধ্যে রিকোয়েস্টের ল্যাটেন্সি বা সাইজকে বিভিন্ন বাকেটে (যেমন: `<50ms`, `<100ms`, `<500ms`, `<2s`) ভাগ করে গণনা করে। এটি p50, p95, p99 পারসেন্টাইল বের করতে সাহায্য করে:
   $$\text{p99 Latency} = \text{histogram\_quantile}(0.99, \, \text{sum}(\text{rate}(\text{http\_request\_duration\_seconds\_bucket}[5m])) \text{ by } (\text{le}))$$
4. **Summary**: ক্লায়েন্ট অ্যাপ্লিকেশনের ভেতরে সরাসরি কোয়ান্টাইল গণনা করে। এটি অত্যন্ত নিখুঁত হলেও একাধিক সার্ভার নোডের মাঝে সমষ্টি (Aggregation) করা যায় না।

---

### খ. স্ট্রাকচার্ড লগিং (Structured JSON Logging)
সাধারণ স্ট্রিং লগ (যেমন: `log.Info("User " + userId + " paid " + amount)`) বড় ডিস্ট্রিবিউটেড সিস্টেমে সার্চ বা ইন্ডেক্স করা অসম্ভব। আধুনিক সিস্টেমে বাধ্যতামূলকভাবে **স্ট্রাকচার্ড JSON লগিং** ব্যবহার করতে হয়:

```json
{
  "timestamp": "2026-09-14T11:00:00.123Z",
  "level": "ERROR",
  "message": "Payment transaction failed",
  "service.name": "payment-service",
  "deployment.environment": "production",
  "user.id": "usr_98741",
  "order.amount": 149.99,
  "error.code": "CARD_EXPIRED",
  "trace_id": "4bf92f3577b34da6a3ce929d0e0e4736",
  "span_id": "00f067aa0ba902b7"
}
```

---

## ২. ওপেন-টেলিমেট্রি (OpenTelemetry - OTel) আর্কিটেকচার এবং মানদণ্ড

ওপেন-টেলিমেট্রির পূর্বে কোম্পানিগুলো বিভিন্ন ভেন্ডর এজেন্ট (যেমন: Datadog agent, New Relic) অথবা পরস্পরবিরোধী লাইব্রেরির (OpenTracing বনাম OpenCensus) ফাঁদে আটকে থাকত।

**OpenTelemetry (OTel)** হলো CNCF-এর অনুমোদিত ইন্ডাস্ট্রি-স্ট্যান্ডার্ড যা একক ও ভেন্ডর-নিরপেক্ষ API, SDK এবং কালেক্টর পাইপলাইন প্রদান করে।

![OpenTelemetry Collector Architecture](images/otel-collector-architecture.svg)

### API বনাম SDK বিভাজন
- **OTel API**: অ্যাপ্লিকেশন কোডে ট্র্যাকিং শুরু করার ইন্টারফেস সংজ্ঞায়িত করে (যেমন: স্প্যান তৈরি, কাউন্টার বৃদ্ধি)। এতে কোনো এক্সটার্নাল ডিপেন্ডেন্সি থাকে না এবং যেকোনো শেয়ার্ড লাইব্রেরিতে নিরাপদে ব্যবহার করা যায়।
- **OTel SDK**: API-এর বাস্তবায়ন যা কনফিগারেশন, মেমোরি ম্যানেজমেন্ট, ব্যাচিং এবং নেটওয়ার্ক ডাটা পাঠানো পরিচালনা করে। এটি কেবল অ্যাপ্লিকেশনের মেইন এন্ট্রি পয়েন্টে (`Program.cs`) কনফিগার করা হয়।

### ওপেন-টেলিমেট্রি কালেক্টর (OTel Collector) পাইপলাইন
কালেক্টর একটি আউট-অফ-প্রসেস প্রক্সি হিসেবে একাধিক সার্ভিসের টেলিমেট্রি গ্রহণ করে প্রসেস করে বিভিন্ন ব্যাকএন্ডে পৌঁছে দেয়। এর ৩টি প্রধান ধাপ রয়েছে:

1. **Receivers (রিসিভার)**:
   - বিভিন্ন প্রোটোকলে টেলিমেট্রি গ্রহণ করে (OTLP gRPC পোর্ট `4317`, OTLP HTTP পোর্ট `4318`, Prometheus scrape, Jaeger)।
   - ডাটাকে স্ট্যান্ডার্ড OTel ইন্টারনাল ফরমেটে (`pdata`) রূপান্তর করে।
2. **Processors (প্রসেসর)**:
   - **`memory_limiter`**: প্রথম প্রসেসর। ট্রাফিক স্পাইকের সময় কালেক্টরের র‍্যাম সীমা অতিক্রম করলে ডাটা রিফিউজ করে কালেক্টরকে মেমোরি ক্র্যাশ (OOM) থেকে বাঁচায়।
   - **`batch`**: প্রতি ২০০ মিলিমিটার পর পর বা ৮১৯২টি আইটেম একত্রিত করে নেটওয়ার্ক কল কমায় এবং থ্রুপুট বাড়ায়।
   - **`transform`**: সংবেদনশীল PII ডেটা (পাসওয়ার্ড, কার্ড নম্বর) মাস্ক বা মুছে ফেলে।
   - **`tail_sampling`**: ১০০% এরর এবং উচ্চ ল্যাটেন্সির ট্রেস সংরক্ষণ করে সাধারণ সফল ট্রেস ড্রপ করে দেয়।
3. **Exporters (এক্সপোর্টার)**:
   - ইন্টারনাল টেলিমেট্রিকে বিভিন্ন ব্যাকএন্ডের নিজস্ব ফরমেটে রূপান্তর করে পাঠায় (Prometheus-এ `prometheusremotewrite`, Grafana Tempo-তে `otlp`, Grafana Loki-তে `otlphttp`)।

---

## ৩. ডিস্ট্রিবিউটেড কনটেক্সট প্রোপাগেশন এবং W3C TraceContext

যখন একটি রিকোয়েস্ট ১০টি মাইক্রোসার্ভিসের মধ্য দিয়ে যায়, তখন সর্বশেষ সার্ভিসটি কীভাবে জানতে পারে যে এটি কোন ক্লায়েন্টের ট্রানজ্যাকশন? **কনটেক্সট প্রোপাগেশন (Context Propagation)** নেটওয়ার্ক বাউন্ডারির মধ্য দিয়ে HTTP হেডার বা gRPC মেটাডাটার মাধ্যমে ট্র্যাকিং স্টেট সরবরাহ করে।

![Distributed Tracing & Context Propagation](images/distributed-tracing-context-propagation.svg)

### W3C TraceContext স্পেসিফিকেশন
W3C কর্তৃক অনুমোদিত `traceparent` HTTP হেডারটি আন্তর্জাতিক মাইক্রোসার্ভিস ইকোসিস্টেমে ডিস্ট্রিবিউটেড ট্র্যাকিং পরিচালনা করে:

```
traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01
             │  └──────────────┬───────────────┘ └───────┬──────┘ └─┬─┘
          Version           Trace ID                 Parent Span ID Flags
```

- **Version (`00`)**: বর্তমান W3C স্পেসিফিকেশন ভার্সন ($২$ হেক্সাডেসিমাল ক্যারেক্টার)।
- **Trace ID (`4bf92f3577b34da6a3ce929d0e0e4736`)**: ১৬-বাইটের ($৩২$ হেক্স ক্যারেক্টার) বিশ্বজনীন অনন্য আইডেন্টিফায়ার। এটি পুরো রিকোয়েস্টের প্রতিটি মাইক্রোসার্ভিসে অভিন্ন থাকে!
- **Parent Span ID (`00f067aa0ba902b7`)**: ৮-বাইটের ($১৬$ হেক্স ক্যারেক্টার) কলার সার্ভিসের নির্দিষ্ট এক্সিকিউশন স্প্যান আইডি।
- **Trace Flags (`01`)**: ৮-বিট ফ্ল্যাগ। `01` নির্দেশ করে ট্রেসটি স্টোরেজে সংরক্ষণ (Sampled) করা হয়েছে।

### ব্যাগেজ (Baggage API) বনাম ট্রেস অ্যাট্রিবিউট
- **Trace Attributes**: একটি নির্দিষ্ট স্প্যানের ভেতরে সীমাবদ্ধ থাকে; ডাউনস্ট্রিমে যায় না।
- **Baggage (`baggage` হেডার)**: কি-ভ্যালু পেয়ার যা ডাউনস্ট্রিমের **প্রতিটি** সার্ভিসে ভ্রমণ করে (যেমন: `baggage: tenantId=nike,userTier=vip`)।
  > [!WARNING]
  > ব্যাগেজ প্লেইন টেক্সট হেডারে যায়, তাই এতে কখনই পাসওয়ার্ড, অথেন্টিকেশন টোকেন বা সংবেদনশীল ডেটা রাখা যাবে না!

### লগ কোরিলেশন (Log Correlation)
Serilog-এর মতো ইঞ্জিনে `TraceId` স্বয়ংক্রিয়ভাবে প্রতিটি লগ লাইনে ইনজেক্ট করলে Grafana ড্যাশবোর্ডে এক ক্লিকেই ল্যাটেন্সি গ্রাফ থেকে নির্দিষ্ট এরর লগে জাম্প করা যায়:

$$\text{লগ লাইন} \xrightarrow{\text{TraceId}} \text{ডিস্ট্রিবিউটেড ট্রেস গ্রাফ} \xrightarrow{\text{SpanId}} \text{নির্দিষ্ট মেথডের এরর স্ট্যাক ট্রেস}$$

---

## ৪. SRE অবজারভেবিলিটি ফ্রেমওয়ার্ক এবং মেথডোলজি

সিস্টেম ডিজাইন ইন্টারভিউতে রিকোয়েস্ট-ভিত্তিক মাইক্রোসার্ভিস বনাম আন্ডারলাইং ইনফ্রাস্ট্রাকচারের জন্য সুনির্দিষ্ট ফ্রেমওয়ার্ক বেছে নিতে হয়:

```
                      অবজারভেবিলিটি ফ্রেমওয়ার্ক নির্বাচন
                                      │
              ┌───────────────────────┴───────────────────────┐
              ▼                                               ▼
     রিকোয়েস্ট চালিত মাইক্রোসার্ভিস                       হোস্ট ও ইনফ্রাস্ট্রাকচার
         (The RED Method)                                (The USE Method)
 ├── Rate: রিকোয়েস্ট পার সেকেন্ড (RPS)            ├── Utilization: ব্যবহৃত ক্ষমতার শতকরা হার
 ├── Errors: ব্যর্থ রিকোয়েস্ট সংখ্যা             ├── Saturation: কিউতে আটকে থাকা রিকোয়েস্ট
 └── Duration: রিকোয়েস্টের ল্যাটেন্সি             └── Errors: হার্ডওয়্যার ও নেটওয়ার্ক ত্রুটি
```

### ১. দ্য ৪ গোল্ডেন সিগন্যালস (Google SRE)
1. **Latency (বিলম্ব)**: রিকোয়েস্ট প্রসেস করতে কত সময় লাগছে। সফল ও ব্যর্থ রিকোয়েস্টের ল্যাটেন্সি আলাদাভাবে মাপুন।
2. **Traffic (ট্রাফিক)**: সিস্টেমে চাহিদার পরিমাণ (যেমন: HTTP RPS, নেটওয়ার্ক I/O)।
3. **Errors (ত্রুটি)**: ব্যর্থ রিকোয়েস্টের অনুপাত (HTTP 500 বা 2s টাইমআউট অতিক্রম)।
4. **Saturation (সম্পৃক্তি)**: সিস্টেম কতটা পূর্ণ। সীমাবদ্ধ রিসোর্স (CPU, RAM, থ্রেড পুল কিউ) পরিমাপ করে।

### ২. দ্য RED মেথড (Microservices Focus)
- **Rate**: প্রতি সেকেন্ডে সার্ভিসে আসা মোট রিকোয়েস্ট সংখ্যা।
- **Errors**: সেই রিকোয়েস্টগুলোর মধ্যে কতটি ফেইল করেছে।
- **Duration**: রিকোয়েস্টগুলো সম্পন্ন হতে কত সময় লেগেছে (p50, p95, p99 পারসেন্টাইল)।

### ৩. দ্য USE মেথড (Infrastructure & Hardware Focus)
- **Utilization**: রিসোর্সটি ঠিক কত সময় ধরে কাজে ব্যস্ত ছিল (যেমন: ডিস্ক ৯০% ব্যস্ত)।
- **Saturation**: অতিরিক্ত কাজের কারণে কিউতে কতটা রিকোয়েস্ট জমা আছে (যেমন: CPU রান কিউ)।
- **Errors**: ডিভাইস এরর ইভেন্ট সংখ্যা (ড্রপড নেটওয়ার্ক প্যাকেট, ডিস্ক রিড এরর)।

---

## ৫. স্যাম্পলিং স্ট্র্যাটেজি এবং হাই-কার্ডিনালিটি ব্যবস্থাপনা (Sampling & High-Cardinality)

প্রতি সেকেন্ডে লক্ষাধিক রিকোয়েস্ট হ্যান্ডেল করা সিস্টেমে শতভাগ ট্রেস ও মেট্রিক্স সংরক্ষণ করতে গেলে বিশাল স্টোরেজ খরচ হয় এবং নেটওয়ার্ক ব্যান্ডউইথ ধ্বংস হয়ে যায়।

![Trace Sampling Strategies](images/sampling-strategies.svg)

### কার্ডিনালিটি বিস্ফোরণ সমস্যা (The Cardinality Explosion)
**কার্ডিনালিটি** হলো একটি মেট্রিক লেবেলের সম্ভাব্য অনন্য মানের সংখ্যা।
- **লো-কার্ডিনালিটি (নিরাপদ)**: `http_method` (`GET`, `POST`), `status_code` (`200`, `500`), `region` (`us-east`)।
- **হাই-কার্ডিনালিটি (মেট্রিক্সে বিপজ্জনক)**: `user_id`, `order_id`, `email`, `uuid`।
  > [!CAUTION]
  > কখনই `user_id` বা `uuid`-এর মতো হাই-কার্ডিনালিটি মান **Prometheus মেট্রিক লেবেলে** রাখবেন না! ১ কোটি ইউজার থাকলে প্রমিথিউস ১ কোটি টাইম-সিরিজ তৈরি করবে এবং র‍্যাম শেষ হয়ে প্রমিথিউস ডাটাবেজ ক্র্যাশ করবে। এগুলোকে কেবল **লগ** এবং **ট্রেস স্প্যান অ্যাট্রিবিউটে** রাখতে হবে।

### হেড-বেসড বনাম টেইল-বেসড স্যাম্পলিং

| আর্কিটেকচারাল মাত্রা | হেড-বেসড স্যাম্পলিং (Head-Based / SDK) | টেইল-বেসড স্যাম্পলিং (Tail-Based / Collector) |
| :--- | :--- | :--- |
| **সিদ্ধান্ত গ্রহণের মুহূর্ত** | রিকোয়েস্টের একদম শুরুতে (API Gateway / SDK)। | রিকোয়েস্ট সম্পন্ন হওয়ার একদম শেষে (Collector)। |
| **মূল্যায়ন করার মানদণ্ড** | অন্ধ গাণিতিক অনুপাত (যেমন: আগত ট্রাফিকের ৫% স্যাম্পল করো)। | গভীর ফিল্টারিং (HTTP স্ট্যাটাস, ল্যাটেন্সি, কাস্টমার টিয়ার)। |
| **বিরল ত্রুটি দেখার ক্ষমতা** | **দুর্বল**: ৯৫% ড্রপ করা ট্রাফিকের ভেতর পেমেন্ট এরর হলে তা চিরতরে হারিয়ে যায়। | **শতভাগ ($100\%$)**: প্রতিটি HTTP 5xx এরর এবং $>২$ সেকেন্ডের ল্যাটেন্সি ট্রেস শতভাগ সংরক্ষিত হয়। |
| **নেটওয়ার্ক ও মেমোরি খরচ** | **সবচেয়ে কম**: ড্রপ করা রিকোয়েস্টে স্প্যান তৈরি হয় না। | **মাঝারি**: কালেক্টরের মেমোরিতে সাময়িক বাফার রাখতে হয়। |
| **সেরা ব্যবহারের ক্ষেত্র** | সাধারণ অভ্যন্তরীণ ট্রাফিক; চরম স্কেল ($>১$ মিলিয়ন RPS)। | টিয়ার-০ ব্যবসায়িক মাইক্রোসার্ভিস (পেমেন্ট, ই-কমার্স)। |

---

## ৬. অ্যালার্টিং এবং ইনসিডেন্ট গভর্ন্যান্স (Alerting & Incident Governance)

সফল ইঞ্জিনিয়ারিং টিম **অ্যালার্ট ক্লান্তি (Alert Fatigue)** এড়াতে অভ্যন্তরীণ কারণের বদলে সরাসরি ব্যবহারকারী ক্ষতিগ্রস্ত হওয়া উপসর্গের ওপর অ্যালার্ট তৈরি করে।

### সিম্পটম-বেসড বনাম কজ-বেসড অ্যালার্টিং
- **কজ-বেসড অ্যালার্ট (খারাপ অভ্যাস)**: *"সার্ভারের সিপিইউ ৯২% এ পৌঁছেছে!"* (ব্যাকগ্রাউন্ড জবের কারণে এটি স্বাভাবিক হতে পারে; অন-কল ইঞ্জিনিয়ারের ঘুম নষ্ট করে)।
- **সিম্পটম-বেসড অ্যালার্ট (উত্তম অভ্যাস)**: *"গত ৫ মিনিটে কাস্টমার চেকআউট ব্যর্থতার হার ১% ছাড়িয়েছে!"* (ব্যবহারকারী সরাসরি ক্ষতিগ্রস্ত; অবিলম্বে ফিক্স দরকার)।

### মাল্টি-উইন্ডো মাল্টি-বার্ন-রেট অ্যালার্টিং
Google SRE-এর নিয়ম অনুযায়ী স্ট্যাটিক থ্রেশহোল্ডের বদলে **এরর বাজেট পুড়ে যাওয়ার হারের (Burn Rate)** ওপর ভিত্তি করে PagerDuty অ্যালার্ট ট্রিগার করতে হয়:

```
অ্যালার্ট শর্তাবলী:
 ├── শর্ট-উইন্ডো (২ মিনিট) বার্ন রেট অত্যন্ত উচ্চ এবং
 └── লং-উইন্ডো (১ ঘণ্টা) বার্ন রেটও বিপজ্জনক অবস্থায় আছে
      └── তাৎক্ষণিকভাবে অন-কল সিনিয়র ইঞ্জিনিয়ারকে P1 পেজ পাঠানো হয়!
```

---

## ৭. নিরাপত্তা, গোপনীয়তা এবং PII ডেটা মাস্কিং (Security & PII Redaction)

টেলিমেট্রিতে অসাবধানতাবশত পাসওয়ার্ড, ক্রেডিট কার্ড নম্বর বা বিয়ারার টোকেন সংরক্ষণ করা আইনত দণ্ডনীয় (GDPR/PCI-DSS লঙ্ঘন)।

### মাল্টি-লেয়ার রিডাকশন পাইপলাইন
1. **অ্যাপ্লিকেশন লেভেলে স্যানিটাইজেশন**: Serilog কনফিগারেশনে `password`, `ssn`, `credit_card` কি-গুলো মাস্ক করা হয়।
2. **কালেক্টরে ট্রান্সফর্ম প্রসেসর**: সেন্ট্রালাইজড OTTL রুল দিয়ে সব মাইক্রোসার্ভিসের লগ ও ট্রেস থেকে রেগুলার এক্সপ্রেশন দিয়ে কার্ড নম্বর `[REDACTED_PAN]` দ্বারা প্রতিস্থাপন করা হয়:

```yaml
# otel-collector-config.yaml
processors:
  transform:
    error_mode: ignore
    log_statements:
      - context: log
        statements:
          - replace_pattern(body, "(\\d{4}[- ]?){3}\\d{4}", "[REDACTED_PAN]")
          - set(attributes["http.request.header.authorization"], "[REDACTED]")
```

---

## ৮. আর্কিটেকচারাল স্ট্যান্ডার্ড (.NET / C# ফোকাস)

### সুবিধাসমূহ:
- **জিরো-অ্যালোকেশন নেটিভ ইঞ্জিন**: .NET 8/9-এ ট্রেসিং সরাসরি `System.Diagnostics.ActivitySource` এবং মেট্রিক্স `System.Diagnostics.Metrics.Meter`-এর মাধ্যমে রানটাইমে বিল্ট-ইন থাকে।
- **স্বয়ংক্রিয় প্রোপাগেশন**: `HttpClient` স্বয়ংক্রিয়ভাবে W3C `traceparent` হেডার পাঠায় এবং ASP.NET Core স্বয়ংক্রিয়ভাবে তা গ্রহণ করে কনটেক্সট তৈরি করে।

### দুর্বলতা ও ট্রেড-অফ:
- **কালেক্টর মেমোরি ব্যালান্স**: টেইল-বেসড স্যাম্পলিং চালানোর সময় কালেক্টর কন্টেইনারে পর্যাপ্ত র‍্যাম ও `memory_limiter` কনফিগার করতে হয়।

### লক্ষ্যযুক্ত প্রযুক্তি ও .NET-এর জন্য NuGet প্যাকেজ:
- `OpenTelemetry.Extensions.Hosting` (মূল রানটাইম ইন্টিগ্রেশন)
- `OpenTelemetry.Instrumentation.AspNetCore` (HTTP রিকোয়েস্ট অটো-ট্রেসিং)
- `OpenTelemetry.Instrumentation.Http` (`HttpClient` ডব্লিউথ্রি-সি হেডার ইনজেকশন)
- `OpenTelemetry.Exporter.OpenTelemetryProtocol` (হাই-পারফরম্যান্স OTLP এক্সপোর্টার)
- `Serilog.AspNetCore` ও `Serilog.Enrichers.Span` (ট্রেস কোরিলেশন সমৃদ্ধ স্ট্রাকচার্ড লগিং)

---

### প্রোডাকশন-গ্রেড C# কোড উদাহরণ ১: পূর্ণাঙ্গ OpenTelemetry পাইপলাইন (.NET 8/9)

```csharp
// Program.cs - .NET 8/9-এ প্রোডাকশন-গ্রেড OpenTelemetry পাইপলাইন
using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// ১. কাস্টম ইন্সট্রুমেন্টেশন সোর্স
const string ServiceName = "OrderProcessingService";
const string ServiceVersion = "1.0.0";

// ActivitySource ডিস্ট্রিবিউটেড ট্রেসিং স্প্যান তৈরি করে
var appActivitySource = new ActivitySource(ServiceName, ServiceVersion);

// Meter কাস্টম বিজনেস মেট্রিক্স (Counter, Histogram) তৈরি করে
var appMeter = new Meter(ServiceName, ServiceVersion);
var orderCounter = appMeter.CreateCounter<long>("orders.completed.count", description: "মোট সফল অর্ডার সংখ্যা");
var orderDurationHistogram = appMeter.CreateHistogram<double>("orders.processing.duration.ms", unit: "ms");

// ২. ইউনিফাইড OpenTelemetry কনফিগারেশন
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: ServiceName, serviceVersion: ServiceVersion)
        .AddAttributes(new[]
        {
            new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName),
            new KeyValuePair<string, object>("host.name", Environment.MachineName)
        }))
    .WithTracing(tracing => tracing
        .AddSource(ServiceName) // কাস্টম স্প্যান গ্রহণ করে
        .AddAspNetCoreInstrumentation(opts => opts.RecordException = true) // ইনকামিং HTTP অটো-ট্রেসিং
        .AddHttpClientInstrumentation() // আউটগোয়িং কলে traceparent হেডার পাঠায়
        .AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317");
            opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }))
    .WithMetrics(metrics => metrics
        .AddMeter(ServiceName) // কাস্টম কাউন্টার/হিস্টোগ্রাম সংগ্রহ করে
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation() // মেমোরি, GC এবং থ্রেড পুল মেট্রিক্স সংগ্রহ করে
        .AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317");
            opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }));

var app = builder.Build();

app.MapPost("/api/orders", async (OrderRequest request) =>
{
    var stopwatch = Stopwatch.StartNew();

    // প্যারেন্ট W3C ট্রানজ্যাকশনের সাথে যুক্ত বিজনেস স্প্যান শুরু
    using var activity = appActivitySource.StartActivity("ProcessOrderTransaction");
    activity?.SetTag("order.id", request.OrderId);
    activity?.SetTag("customer.id", request.CustomerId);

    try
    {
        // ডাটাবেজ কাজ ও বিজনেস লজিক এক্সিকিউশন
        await Task.Delay(Random.Shared.Next(20, 80));

        // মেট্রিক রেকর্ড করা হলো
        orderCounter.Add(1, new KeyValuePair<string, object?>("customer.tier", request.Tier));
        stopwatch.Stop();
        orderDurationHistogram.Record(stopwatch.ElapsedMilliseconds);

        return Results.Ok(new { Status = "Success", OrderId = request.OrderId });
    }
    catch (Exception ex)
    {
        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        activity?.RecordException(ex);
        throw;
    }
});

app.Run();

public record OrderRequest(string OrderId, string CustomerId, string Tier);
```

---

### প্রোডাকশন-গ্রেড C# কোড উদাহরণ ২: ট্রেস কোরিলেশন সহ Serilog স্ট্রাকচার্ড লগিং

```csharp
// Program.cs - Serilog ও W3C TraceId/SpanId স্বয়ংক্রিয় সংযুক্তি
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Serilog কনফিগারেশন: প্রতিটি লগে TraceId এবং SpanId ইনজেক্ট করা হবে
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "OrderService")
    // নেটিভ OpenTelemetry স্প্যান কোরিলেশন যুক্ত করে
    .Enrich.WithSpan() 
    .WriteTo.Console(new CompactJsonFormatter()) // Loki/Fluentd-এর জন্য কম্প্যাক্ট JSON
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.MapGet("/api/checkout", (ILogger<Program> logger) =>
{
    // Serilog স্বয়ংক্রিয়ভাবে Activity.Current.TraceId লগে বসিয়ে দেয়
    logger.LogInformation("Processing checkout transaction for tenant {TenantId}", "NikeCorp");
    return Results.Ok(new { Status = "Checked out" });
});

app.Run();
```

---

## ৯. সিস্টেম ডিজাইন ইন্টারভিউ ডিসিশন ফ্রেমওয়ার্ক (Observability Decision Tree)

ইন্টারভিউতে অবজারভেবিলিটি সম্পর্কিত প্রশ্নের উত্তরে নিচের সিদ্ধান্ত কাঠামোটি অনুসরণ করুন:

```
অবজারভেবিলিটি আর্কিটেকচার সিদ্ধান্ত কাঠামো
 ├── মূল পরিচালনগত প্রশ্নটি কী?
 │    ├── "সিস্টেমটি কি এই মুহূর্তে সুস্থ আছে?" ──► Metrics (Prometheus / Grafana) + The RED Method
 │    ├── "মাল্টি-হপ নেটওয়ার্কের ঠিক কোথায় বিলম্ব হচ্ছে?" ──► Distributed Tracing (Tempo / Jaeger) + W3C TraceContext
 │    ├── "এই নির্দিষ্ট ট্রানজ্যাকশনটি ক্র্যাশ করল কেন?" ──► Structured JSON Logging (Loki / Serilog) via TraceId
 │    └── "প্রোডাকশনে কোডের কোন মেথডটি বেশি সিপিইউ পোড়াচ্ছে?" ──► Continuous Profiling (Grafana Pyroscope)
 │
 ├── টেলিমেট্রির উচ্চ ভলিউম ও স্টোরেজ খরচ কীভাবে নিয়ন্ত্রণ করবেন?
 │    ├── অত্যন্ত বিশাল স্কেল (>১ লাখ RPS) ও নেটওয়ার্ক বাধা? ──► হেড-বেসড প্রবাবিলিস্টিক স্যাম্পলিং (Ingress)
 │    └── প্রতিটি ত্রুটি ও উচ্চ ল্যাটেন্সি ধরা বাধ্যতামূলক? ──► টেইল-বেসড স্যাম্পলিং (OTel Collector)
 │
 ├── মেট্রিক্সের কার্ডিনালিটি বিস্ফোরণ কীভাবে রোধ করবেন?
 │    ├── লো-কার্ডিনালিটি (HTTP Method, Status Code, Region) ──► Prometheus Metric Labels-এ রাখুন
 │    └── হাই-কার্ডিনালিটি (User ID, Order ID, UUID) ──► কঠোরভাবে Trace Attributes ও Logs-এ রাখুন
 │
 └── অন-কল অ্যালার্ট কীভাবে সাজাবেন?
      ├── CPU বা মেমোরি থ্রেশহোল্ডে অ্যালার্ট করবেন? ──► না (অ্যালার্ট ক্লান্তি ও ভুয়া অ্যালার্ম তৈরি করে)
      └── গ্রাহকের দুর্ভোগ ও SLO বার্ন রেটে অ্যালার্ট করবেন? ──► হ্যাঁ (মাল্টি-উইন্ডো মাল্টি-বার্ন PagerDuty পেজ)
```

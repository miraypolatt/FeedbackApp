##  Kullanılan Teknolojiler

- **Frontend**: ReactJS (Vite ile başlandı)
- **Backend**: ASP.NET Core 8 Web API
- **Queue**: RabbitMQ
- **Database**: MongoDB (Docker ile çalıştırıldı)
- **Containerization**: Docker, Docker Compose


---

## Mimaride Kullanılan Yapılar

Katmanlı mimari tercih edilmiştir. 4 temel katman bulunmaktadır:

### ✅ API Layer
- `POST /api/feedback` endpointi ile form verisini alır.
- Dış dünyaya açık olan katmandır (controller'lar bu katmanda yer alır).

### ✅ Application Layer
- Servis interface'leri ve iş mantığı bu katmanda bulunur.
- Feedback gönderme işlemi burada yönetilir.

### ✅ Domain Layer
- `Feedback` modeli ve `FeedbackDto` gibi temel veri yapıları burada tanımlanır.

### ✅ Infrastructure Layer
- RabbitMQ Publisher / Consumer bağlantıları.
- MongoDB Repository implementasyonu.
- Loglama servisleri.

---

##  RabbitMQ Consumer (WorkerApp)

- RabbitMQ'dan mesajları dinleyen arka plan uygulamasıdır.
- Kuyruktan gelen mesajları deserialize eder.
- Verileri `feedbacks` koleksiyonuna kaydeder.
- Hatalı mesajları `error_logs` koleksiyonuna ayrı olarak loglar.

### 🔍 Mesaj Süreci
- **Validation**: React üzerinde boş alan, isim ve mail kontrolü
1. React formundan veri gelir.
2. API `POST /api/feedback` ile alır.
3. RabbitMQ'ya JSON formatında publish edilir.
4. Consumer bu mesajı alır, deserialize eder.
5. MongoDB'ye başarılı mesajlar kaydedilir.
6. Deserialize edilemeyen veya başarısız işlemler `error_logs` koleksiyonuna yazılır.

> Hataları yakalamak ve takip edebilmek için özel `ErrorLoggerHelper` ve `ErrorLoggerFactory` sınıfları yazıldı. Böylece her loglama işlemi merkezi bir yerden yönetilebiliyor.

---

##  Docker Kurulumu

Tüm bileşenleri tek komutla ayağa kaldırmak için:
docker-compose up --build


Servis	Adres
API	http://localhost:7111
React	http://localhost:5173
RabbitMQ UI	http://localhost:15672
MongoDB	localhost:27017 (Docker içi)

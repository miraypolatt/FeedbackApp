import FeedbackForm from "../components/FeedbackForm";
import "/src/form.css"; 
export default function Home() {
  return (
    <div className="home-container">
      <h1 className="page-title">Kullanıcı Geri Bildirim Formu</h1>
      <FeedbackForm />
    </div>
  );
}

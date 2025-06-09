// // FeedbackForm.jsx
import React, { useState } from "react";
import feedbackService from "/Users/aleyna/Desktop/feedback-app-frontend/src/assets/services/feedbackService"; // doğru import
import "C:/Users/aleyna/Desktop/feedback-app-frontend/src/form.css";

export default function FeedbackForm() {
  const [form, setForm] = useState({ name: "", email: "", message: "" });
  const [loading, setLoading] = useState(false);

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (form.name.trim().length < 2) {
      alert("Lütfen isminizi girin");
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(form.email)) {
      alert("Geçerli bir e-posta adresi girin.");
      return;
    }

    if (form.message.trim().length === 0) {
      alert("Lütfen bir mesaj girin.");
      return;
    }

    try {
      setLoading(true);
      await feedbackService.sendFeedback(form);
      alert("Geri bildiriminiz gönderildi!");
      setForm({ name: "", email: "", message: "" });
    } catch (err) {
      console.error("Gönderim hatası:", err);
      alert("Bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="form-container" onSubmit={handleSubmit}>
      <h2>Geri Bildirim Formu</h2>
      <div className="form-group">
        <label>Ad</label>
        <input type="text" name="name" value={form.name} onChange={handleChange} />
      </div>
      <div className="form-group">
        <label>Email</label>
        <input type="email" name="email" value={form.email} onChange={handleChange} />
      </div>
      <div className="form-group">
        <label>Mesaj</label>
        <textarea name="message" value={form.message} onChange={handleChange} />
      </div>
      <button className="submit-button" type="submit" disabled={loading}>
        {loading ? "Gönderiliyor..." : "Gönder"}
      </button>
    </form>
  );
}


// src/services/feedbackService.js
import axios from "axios";

const API_URL = "/api/feedback";

const sendFeedback = (data) => {
  return axios.post(API_URL, data);
};

export default {
  sendFeedback,
};

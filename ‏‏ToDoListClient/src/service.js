import axios from 'axios';

axios.defaults.baseURL = 'http://localhost:5007';
// הוספת Interceptor לתפיסת שגיאות בתגובה
axios.interceptors.response.use(
  response => response, // אם התגובה תקינה החזר אותה כמו שהיא
  error => {
    // רישום השגיאה ללוג
    console.error('Error in Axios response:', error.message || error);

    // החזרת השגיאה כך שניתן יהיה לטפל בה בקוד
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`/items`)    
    return result.data;
  },

  addTask: async(name)=>{

    const result = await axios.post(`/items`,{Name:name})   
    return result.data;
  },
  setCompleted: async (id, IsComplete) => {
    const result = await axios.put(`/items/${id}`, {IsComplete: IsComplete});   
    return result.data;
  }
,
  deleteTask:async(id)=>{
    console.log('deleteTask')
    const result = await axios.delete(`/items/${id}`)   
  }
};

import axios from 'axios';

const apiUrl = "http://localhost:5020"

axios.defaults.baseURL = apiUrl;

axios.interceptors.response.use(
  response => response,
  error => {
    console.error('API Error:', error.response ? error.response.data : error.message);
    return Promise.reject(error);
  }
);

// axios.interceptors.response.use(
//   response => response,
//   error => {
//     console.error('API Error:', {
//       message: error.message,
//       status: error.response?.status,
//       data: error.response?.data,
//       headers: error.response?.headers
//     });
//     return Promise.reject(error);
//   }
// );


export default {
  getTasks: async () => {
    const result = await axios.get('/items'); // במקום '/tasks'

    return result.data?result.data:[];
  },

  addTask: async (name) => {

    await axios.post('/items', { name: name, isComplete: false });

  },
  
  setCompleted: async (id, isComplete,name) => {
    await axios.put(`/items/${id}`, {name:name,isComplete: isComplete });
  },

  deleteTask: async (id) => {
    await axios.delete(`/items/${id}`);
  }
};


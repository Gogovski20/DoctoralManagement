import apiClient from './apiClient';

export const authService = {
  register: async (fullName, email, password, role) => {
    const response = await apiClient.post('/Auth/register', {
      fullName,
      email,
      password,
      role,
    });
    return response.data;
  },

  login: async (email, password) => {
    const response = await apiClient.post('/Auth/login', {
      email,
      password,
    });
    if (response.data.success) {
      localStorage.setItem('authToken', response.data.token);
      localStorage.setItem('user', JSON.stringify({
        email: response.data.email,
        token: response.data.token,
      }));
    }
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
  },

  getCurrentUser: () => {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },
};

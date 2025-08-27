import React, { createContext, useContext, useState, useEffect } from 'react';
import { authService } from '../services/authService';

export const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(() => localStorage.getItem('token'));
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (token) {
      setUser(JSON.parse(localStorage.getItem('user')));
    }
    setLoading(false);
  }, [token]);


  const login = async (email, password) => {
    const data = await authService.login(email, password);
    if (data && data.success && data.token) {
      setToken(data.token);
      setUser(data.user);
      return { success: true };
    }
    return { success: false, message: data?.message || 'Erro desconhecido' };
  };


  const registrar = async (nome, email, senha) => {
    try {
      const data = await authService.registrar(nome, email, senha);
      if (data && data.success && data.token) {
        setToken(data.token);
        setUser(data.user);
      }
      return data;
    } catch (error) {
      return { success: false, message: error.message };
    }
  };


  const logout = () => {
    setUser(null);
    setToken(null);
    authService.logout();
  };

  return (
    <AuthContext.Provider value={{ user, token, loading, login, logout, registrar, isAuthenticated: !!token }}>
      {children}
    </AuthContext.Provider>
  );
};

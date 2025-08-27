import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'

// StrictMode removido para evitar requisições duplicadas em desenvolvimento
createRoot(document.getElementById('root')).render(
  <App />
)

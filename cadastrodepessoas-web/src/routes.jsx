
import { Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/auth/Login';
import Registrar from './pages/auth/Registrar';
import Dashboard from './pages/Dashboard';
import ListarPessoas from './pages/pessoa/ListarPessoas';
import CriarPessoa from './pages/pessoa/CriarPessoa';
import EditarPessoa from './pages/pessoa/EditarPessoa';
import DetalharPessoa from './pages/pessoa/DetalharPessoa';
import MeuPerfil from './pages/auth/MeuPerfil';
import PaginaNaoEncontrada from './pages/PaginaNaoEncontrada';
import SessaoExpirada from './pages/SessaoExpirada';
import ErroServidor from './pages/ErroServidor';
import ErroConexao from './pages/ErroConexao';
import { useAuth } from './hooks/useAuth';

function PrivateRoute({ children }) {
  const { isAuthenticated, loading } = useAuth();
  
  if (loading) {
    return null; // ou um spinner
  }
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  return children;
}


const AppRoutes = () => (
  <Routes>
    <Route path="/login" element={<Login />} />
    <Route path="/registrar" element={<Registrar />} />
    <Route path="/sessao-expirada" element={<SessaoExpirada />} />
    <Route path="/erro-servidor" element={<ErroServidor />} />
    <Route path="/erro-conexao" element={<ErroConexao />} />
    <Route
      path="/"
      element={
        <PrivateRoute>
          <Dashboard />
        </PrivateRoute>
      }
    />
    <Route
      path="/pessoas"
      element={
        <PrivateRoute>
          <ListarPessoas />
        </PrivateRoute>
      }
    />
    <Route
      path="/pessoas/criar"
      element={
        <PrivateRoute>
          <CriarPessoa />
        </PrivateRoute>
      }
    />
    <Route
      path="/pessoas/editar/:id"
      element={
        <PrivateRoute>
          <EditarPessoa />
        </PrivateRoute>
      }
    />
    <Route
      path="/pessoas/detalhes/:id"
      element={
        <PrivateRoute>
          <DetalharPessoa />
        </PrivateRoute>
      }
    />
    <Route
      path="/meu-perfil"
      element={
        <PrivateRoute>
          <MeuPerfil />
        </PrivateRoute>
      }
    />
    <Route path="*" element={<PaginaNaoEncontrada />} />
  </Routes>
);

export default AppRoutes;

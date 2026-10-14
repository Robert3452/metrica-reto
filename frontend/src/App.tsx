import { Navigate, Route, Routes } from "react-router";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import Layout from "./components/Layout";
import LoginPage from "./pages/LoginPage";
import PedidosListPage from "./pages/PedidosListPage";
import PedidoFormPage from "./pages/PedidoFormPage";

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route path="/pedidos" element={<PedidosListPage />} />

          <Route element={<ProtectedRoute adminOnly />}>
            <Route path="/pedidos/nuevo" element={<PedidoFormPage />} />
            <Route path="/pedidos/:id/editar" element={<PedidoFormPage />} />
          </Route>
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/pedidos" replace />} />
    </Routes>
  );
}

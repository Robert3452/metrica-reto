import { Navigate, Outlet, useLocation } from 'react-router'
import { useAuth } from './AuthContext'

export function ProtectedRoute({ adminOnly = false }: { adminOnly?: boolean }) {
  const { session, isAdmin } = useAuth()
  const location = useLocation()

  if (!session) return <Navigate to="/login" replace state={{ from: location }} />
  if (adminOnly && !isAdmin) return <Navigate to="/pedidos" replace />

  return <Outlet />
}
import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { authApi } from '../api/auth'
import { setUnauthorizedHandler } from '../api/client'
import { clearSession, getSession, saveSession } from './session'
import type { Session } from '../types'

interface AuthContextValue {
  session: Session | null
  isAdmin: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Session | null>(() => getSession())
  const queryClient = useQueryClient()

  const logout = useCallback(() => {
    clearSession()
    setSession(null)
    queryClient.clear()               
  }, [queryClient])


  useEffect(() => setUnauthorizedHandler(logout), [logout])


  useEffect(() => {
    if (!session) return
    const ms = new Date(session.expiresIn).getTime() - Date.now()
    const timer = setTimeout(logout, Math.max(ms, 0))
    return () => clearTimeout(timer)
  }, [session, logout])

  const login = useCallback(async (email: string, password: string) => {
    const r = await authApi.login(email, password)
    const s: Session = { token: r.accessToken, expiresIn: r.expiresIn, rol: r.rol, email }
    saveSession(s)
    setSession(s)
  }, [])

  const value = useMemo(
    () => ({ session, isAdmin: session?.rol === 'Admin', login, logout }),
    [session, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth debe usarse dentro de AuthProvider')
  return ctx
}
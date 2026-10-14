import type { Session } from '../types'

const KEY = 'pedidos.session'

export function getSession(): Session | null {
  try {
    const raw = localStorage.getItem(KEY)
    if (!raw) return null
    const session = JSON.parse(raw) as Session
    if (new Date(session.expiresIn).getTime() <= Date.now()) {
      localStorage.removeItem(KEY)
      return null
    }
    return session
  } catch {
    return null
  }
}

export const saveSession = (s: Session) => localStorage.setItem(KEY, JSON.stringify(s))
export const clearSession = () => localStorage.removeItem(KEY)
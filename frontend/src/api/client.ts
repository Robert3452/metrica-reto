import axios from 'axios'
import { getSession } from '../auth/session'
import type { ProblemDetails } from '../types'

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 10_000,
})

let onUnauthorized: () => void = () => {}
export const setUnauthorizedHandler = (fn: () => void) => { onUnauthorized = fn }

api.interceptors.request.use(config => {
  const session = getSession()
  if (session) config.headers.Authorization = `Bearer ${session.token}`
  return config
})

api.interceptors.response.use(
  response => response,
  error => {
    const isLogin = error.config?.url?.includes('/auth/login')
    if (error.response?.status === 401 && !isLogin) onUnauthorized()
    return Promise.reject(error)
  },
)

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError<ProblemDetails>(error)) {
    if (!error.response) return 'No se pudo conectar con el servidor.'
    const { status, data } = error.response
    if (status === 403) return 'No tienes permisos para esta acción.'
    if (status === 429) return 'Demasiadas solicitudes. Intenta en unos segundos.'
    return data?.detail ?? data?.title ?? 'Ocurrió un error inesperado.'
  }
  return 'Ocurrió un error inesperado.'
}
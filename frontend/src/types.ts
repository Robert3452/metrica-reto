export type Rol = 'Admin' | 'User'
export type EstadoPedido = 'Pendiente' | 'EnProceso' | 'Enviado' | 'Entregado' | 'Cancelado'

export interface LoginResponse {
  accessToken: string
  expiresIn: string
  rol: Rol
}

export interface Session {
  token: string
  expiresIn: string
  rol: Rol
  email: string
}

export interface Pedido {
  id: number
  numeroPedido: string
  cliente: string
  total: number
  estado: EstadoPedido
  fecha: string
}

export interface PedidoInput {
  cliente: string
  total: number
}

export interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
}
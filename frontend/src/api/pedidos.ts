import { api } from "./client";
import type { Pedido, PedidoInput } from "../types";

export const pedidosApi = {
  list: () => api.get<Pedido[]>("/api/pedidos").then((r) => r.data),
  get: (id: number) =>
    api.get<Pedido>(`/api/pedidos/${id}`).then((r) => r.data),
  create: (data: PedidoInput) =>
    api.post<Pedido>("/api/pedidos", data).then((r) => r.data),
  update: (id: number, data: PedidoInput) =>
    api.put<Pedido>(`/api/pedidos/${id}`, data).then((r) => r.data),
  remove: (id: number) => api.delete(`/api/pedidos/${id}`),
};

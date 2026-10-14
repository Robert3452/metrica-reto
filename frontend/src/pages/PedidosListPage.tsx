import { Link } from 'react-router'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { pedidosApi } from '../api/pedidos'
import { getErrorMessage } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import type { EstadoPedido, Pedido } from '../types'
import { ErrorBoundary } from '../components/ErrorBoundary'

const money = new Intl.NumberFormat('es-PE', { style: 'currency', currency: 'PEN' })
const fecha = new Intl.DateTimeFormat('es-PE', { dateStyle: 'medium', timeStyle: 'short' })

const estadoUI: Record<EstadoPedido, { label: string; className: string }> = {
  Pendiente: { label: 'Pendiente',  className: 'bg-amber-100 text-amber-800' },
  EnProceso: { label: 'En proceso', className: 'bg-blue-100 text-blue-800' },
  Enviado:   { label: 'Enviado',    className: 'bg-indigo-100 text-indigo-800' },
  Entregado: { label: 'Entregado',  className: 'bg-green-100 text-green-800' },
  Cancelado: { label: 'Cancelado',  className: 'bg-slate-200 text-slate-700' },
}

function getEstadoUI(estado: string) {
  return estadoUI[estado as EstadoPedido] ?? { label: estado, className: 'bg-slate-100 text-slate-700' }
}

function formatFecha(valor: string) {
  const d = new Date(valor)
  return Number.isNaN(d.getTime()) ? '-' : fecha.format(d)
}

function formatTotal(valor: number) {
  return typeof valor === 'number' ? money.format(valor) : '-'
}

export default function PedidosListPage() {
  const { isAdmin } = useAuth()
  const queryClient = useQueryClient()

  const { data, isLoading, isError, error, refetch } = useQuery({
    queryKey: ['pedidos'],
    queryFn: pedidosApi.list,
  })

  const eliminar = useMutation({
    mutationFn: pedidosApi.remove,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['pedidos'] }),
  })

  function handleDelete(p: Pedido) {
    if (window.confirm(`¿Eliminar el pedido ${p.numeroPedido}? Esta acción no se puede deshacer.`)) {
      eliminar.mutate(p.id)
    }
  }

  const lista = Array.isArray(data) ? data : []

  return (
    <section className="space-y-4">
      {/* Cabecera: siempre visible */}
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Pedidos</h1>
        {isAdmin && (
          <Link to="/pedidos/nuevo" className="rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800">
            Nuevo pedido
          </Link>
        )}
      </div>

      {eliminar.isError && (
        <p role="alert" className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">
          {getErrorMessage(eliminar.error)}
        </p>
      )}

      {/* Contenido: solo esta parte cambia según el estado */}
      {isLoading ? (
        <p className="text-slate-500">Cargando pedidos...</p>
      ) : isError ? (
        <div className="rounded-md bg-red-50 p-4 text-sm text-red-700">
          <p>{getErrorMessage(error)}</p>
          <button onClick={() => refetch()} className="mt-2 font-medium underline">Reintentar</button>
        </div>
      ) : lista.length === 0 ? (
        <div className="rounded-xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-500">
          No hay pedidos registrados.
        </div>
      ) : (
        <ErrorBoundary resetKey={String(lista.length)}>
          <PedidosTable
            pedidos={lista}
            isAdmin={isAdmin}
            onDelete={handleDelete}
            deleting={eliminar.isPending}
          />
        </ErrorBoundary>
      )}
    </section>
  )
}

interface PedidosTableProps {
  pedidos: Pedido[]
  isAdmin: boolean
  onDelete: (p: Pedido) => void
  deleting: boolean
}

function PedidosTable({ pedidos, isAdmin, onDelete, deleting }: PedidosTableProps) {
  return (
    <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white shadow-sm">
      <table className="w-full text-left text-sm">
        <thead className="border-b border-slate-200 bg-slate-50 text-xs uppercase text-slate-500">
          <tr>
            <th className="px-4 py-3">Número</th>
            <th className="px-4 py-3">Cliente</th>
            <th className="px-4 py-3 text-right">Total</th>
            <th className="px-4 py-3">Estado</th>
            <th className="px-4 py-3">Fecha</th>
            {isAdmin && <th className="px-4 py-3 text-right">Acciones</th>}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {pedidos.map(p => {
            const estado = getEstadoUI(p.estado)
            return (
              <tr key={p.id} className="hover:bg-slate-50">
                <td className="px-4 py-3 font-mono text-xs">{p.numeroPedido}</td>
                <td className="px-4 py-3">{p.cliente}</td>
                <td className="px-4 py-3 text-right tabular-nums">{formatTotal(p.total)}</td>
                <td className="px-4 py-3">
                  <span className={`rounded-full px-2 py-0.5 text-xs font-medium ${estado.className}`}>
                    {estado.label}
                  </span>
                </td>
                <td className="px-4 py-3 text-slate-500">{formatFecha(p.fecha)}</td>
                {isAdmin && (
                  <td className="px-4 py-3 text-right">
                    <div className="flex justify-end gap-2">
                      <Link to={`/pedidos/${p.id}/editar`} className="rounded-md px-2 py-1 text-slate-700 hover:bg-slate-100">
                        Editar
                      </Link>
                      <button
                        onClick={() => onDelete(p)}
                        disabled={deleting}
                        className="rounded-md px-2 py-1 text-red-600 hover:bg-red-50 disabled:opacity-50"
                      >
                        Eliminar
                      </button>
                    </div>
                  </td>
                )}
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
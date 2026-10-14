import { useState, type FormEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { pedidosApi } from '../api/pedidos'
import { getErrorMessage } from '../api/client'
import type { Pedido, PedidoInput } from '../types'

const inputClass =
  'w-full rounded-md border border-slate-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-slate-900 disabled:bg-slate-100'

export default function PedidoFormPage() {
  const { id } = useParams()
  const pedidoId = id ? Number(id) : null

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ['pedidos', pedidoId],
    queryFn: () => pedidosApi.get(pedidoId!),
    enabled: pedidoId !== null,          
  })

  if (pedidoId !== null && isLoading) return <p className="text-slate-500">Cargando pedido...</p>
  if (pedidoId !== null && isError) {
    return <p className="rounded-md bg-red-50 p-4 text-sm text-red-700">{getErrorMessage(error)}</p>
  }

  return <PedidoForm key={pedidoId ?? 'nuevo'} pedido={data} />
}

type Errors = Partial<Record<keyof PedidoInput, string>>

function PedidoForm({ pedido }: { pedido?: Pedido }) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = pedido !== undefined
  const cerrado = pedido?.estado === 'Entregado' || pedido?.estado === 'Cancelado'

  const [cliente, setCliente] = useState(pedido?.cliente ?? '')
  const [total, setTotal] = useState(pedido ? String(pedido.total) : '')
  const [errors, setErrors] = useState<Errors>({})

  const guardar = useMutation({
    mutationFn: (input: PedidoInput) =>
      isEdit ? pedidosApi.update(pedido.id, input) : pedidosApi.create(input),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['pedidos'] })
      navigate('/pedidos')
    },
  })

  function validar(): Errors {
    const e: Errors = {}
    if (!cliente.trim()) e.cliente = 'El cliente es obligatorio.'
    else if (cliente.trim().length > 150) e.cliente = 'Máximo 150 caracteres.'

    if (!/^\d+(\.\d{1,2})?$/.test(total)) e.total = 'Ingresa un monto válido con hasta 2 decimales.'
    else if (Number(total) <= 0) e.total = 'El total debe ser mayor a cero.'
    return e
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    const errs = validar()
    setErrors(errs)
    if (Object.keys(errs).length > 0) return
    guardar.mutate({ cliente: cliente.trim(), total: Number(total) })
  }

  return (
    <section className="mx-auto max-w-lg space-y-4">
      <h1 className="text-2xl font-semibold">
        {isEdit ? `Editar pedido ${pedido.numeroPedido}` : 'Nuevo pedido'}
      </h1>

      {cerrado && (
        <p className="rounded-md bg-amber-50 px-3 py-2 text-sm text-amber-800">
          Este pedido está {pedido?.estado.toLowerCase()} y ya no se puede modificar.
        </p>
      )}

      {guardar.isError && (
        <p role="alert" className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">
          {getErrorMessage(guardar.error)}
        </p>
      )}

      <form onSubmit={handleSubmit} noValidate className="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
        <label className="block space-y-1">
          <span className="text-sm font-medium">Cliente</span>
          <input
            value={cliente}
            onChange={e => setCliente(e.target.value)}
            disabled={cerrado}
            aria-invalid={!!errors.cliente}
            className={inputClass}
          />
          {errors.cliente && <span className="text-sm text-red-600">{errors.cliente}</span>}
        </label>

        <label className="block space-y-1">
          <span className="text-sm font-medium">Total (S/)</span>
          <input
            inputMode="decimal"
            value={total}
            onChange={e => setTotal(e.target.value)}
            disabled={cerrado}
            aria-invalid={!!errors.total}
            className={inputClass}
          />
          {errors.total && <span className="text-sm text-red-600">{errors.total}</span>}
        </label>

        <div className="flex justify-end gap-2 pt-2">
          <Link to="/pedidos" className="rounded-md border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100">
            Cancelar
          </Link>
          <button
            type="submit"
            disabled={guardar.isPending || cerrado}
            className="rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-50"
          >
            {guardar.isPending ? 'Guardando...' : 'Guardar'}
          </button>
        </div>
      </form>
    </section>
  )
}
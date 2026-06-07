import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { MoreVertical, Pencil, Plus, Car, Loader2, X } from 'lucide-react'
import { veiculosApi } from '../services/veiculosService'
import type { VeiculoResponse, StatusVeiculo, CriarVeiculoRequest, AtualizarVeiculoRequest } from '../types'

const schema = z.object({
  modelo: z.string().min(1, 'Modelo obrigatorio'),
  marca: z.string().min(1, 'Marca obrigatoria'),
  ano: z.coerce.number().min(1900).max(new Date().getFullYear() + 1),
  placa: z.string().min(7, 'Placa invalida').max(8),
})

type FormData = z.infer<typeof schema>

const statusColors: Record<StatusVeiculo, string> = {
  Disponivel: 'bg-green-100 text-green-800',
  EmUso: 'bg-blue-100 text-blue-800',
  EmManutencao: 'bg-yellow-100 text-yellow-800',
  Inativo: 'bg-gray-100 text-gray-600',
}

const statusLabels: Record<StatusVeiculo, string> = {
  Disponivel: 'Disponivel',
  EmUso: 'Em Uso',
  EmManutencao: 'Em Manutencao',
  Inativo: 'Inativo',
}

function VeiculoModal({
  veiculo,
  onClose,
}: {
  veiculo?: VeiculoResponse
  onClose: () => void
}) {
  const qc = useQueryClient()
  const isEditing = Boolean(veiculo)
  const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema) as never,
    defaultValues: veiculo
      ? {
          modelo: veiculo.modelo,
          marca: veiculo.marca,
          ano: veiculo.ano,
          placa: veiculo.placa,
        }
      : undefined,
  })

  const mutation = useMutation({
    mutationFn: async (data: FormData) => {
      if (veiculo) {
        await veiculosApi.atualizar(veiculo.id, data as AtualizarVeiculoRequest)
        return
      }

      await veiculosApi.criar(data as CriarVeiculoRequest)
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['veiculos'] })
      onClose()
    },
  })

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md p-6">
        <div className="flex items-center justify-between mb-5">
          <h2 className="text-lg font-semibold text-slate-800">
            {isEditing ? 'Editar Veiculo' : 'Novo Veiculo'}
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600" aria-label="Fechar">
            <X size={20} />
          </button>
        </div>
        <form onSubmit={handleSubmit(d => mutation.mutate(d))} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Modelo</label>
            <input {...register('modelo')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            {errors.modelo && <p className="text-red-500 text-xs mt-1">{errors.modelo.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Marca</label>
            <input {...register('marca')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            {errors.marca && <p className="text-red-500 text-xs mt-1">{errors.marca.message}</p>}
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Ano</label>
              <input type="number" {...register('ano')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              {errors.ano && <p className="text-red-500 text-xs mt-1">{errors.ano.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Placa</label>
              <input {...register('placa')} placeholder="ABC-1234" className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              {errors.placa && <p className="text-red-500 text-xs mt-1">{errors.placa.message}</p>}
            </div>
          </div>
          {mutation.isError && (
            <p className="text-red-500 text-sm">Erro ao salvar veiculo. Verifique os dados.</p>
          )}
          <div className="flex gap-3 pt-2">
            <button type="button" onClick={onClose} className="flex-1 px-4 py-2 border border-slate-300 rounded-lg text-sm text-slate-700 hover:bg-slate-50">
              Cancelar
            </button>
            <button type="submit" disabled={mutation.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 flex items-center justify-center gap-2">
              {mutation.isPending && <Loader2 size={14} className="animate-spin" />}
              {isEditing ? 'Salvar' : 'Cadastrar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

function VeiculosTable({
  veiculos,
  onEdit,
}: {
  veiculos: VeiculoResponse[]
  onEdit: (veiculo: VeiculoResponse) => void
}) {
  const [menuId, setMenuId] = useState<string | null>(null)

  return (
    <div className="bg-white rounded-xl shadow-sm border border-slate-200">
      {veiculos.length === 0 ? (
        <div className="text-center py-16 text-slate-400">
          <Car size={48} className="mx-auto mb-3 opacity-40" />
          <p>Nenhum veiculo cadastrado</p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-slate-50 border-b border-slate-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Placa</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Veiculo</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Ano</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Km Atual</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-slate-500 uppercase tracking-wider">Acoes</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {veiculos.map(v => (
                <tr key={v.id} className="hover:bg-slate-50">
                  <td className="px-6 py-4 text-sm font-semibold text-slate-800">{v.placa}</td>
                  <td className="px-6 py-4 text-sm text-slate-700">{v.marca} {v.modelo}</td>
                  <td className="px-6 py-4 text-sm text-slate-600">{v.ano}</td>
                  <td className="px-6 py-4 text-sm text-slate-600">
                    {(v.quilometragemAtual ?? 0).toLocaleString('pt-BR')} km
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[v.status]}`}>
                      {statusLabels[v.status]}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="relative inline-flex">
                      <button
                        onClick={() => setMenuId(menuId === v.id ? null : v.id)}
                        className="inline-flex h-8 w-8 items-center justify-center rounded-lg border border-slate-200 text-slate-500 hover:bg-slate-50 hover:text-slate-700"
                        aria-label="Acoes do veiculo"
                      >
                        <MoreVertical size={16} />
                      </button>
                      {menuId === v.id && (
                        <div className="absolute right-0 top-9 z-20 w-36 rounded-lg border border-slate-200 bg-white py-1 text-left shadow-lg">
                          <button
                            onClick={() => {
                              setMenuId(null)
                              onEdit(v)
                            }}
                            className="flex w-full items-center gap-2 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50"
                          >
                            <Pencil size={14} />
                            Editar
                          </button>
                        </div>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

export default function VeiculosPage() {
  const [showModal, setShowModal] = useState(false)
  const [editingVeiculo, setEditingVeiculo] = useState<VeiculoResponse | undefined>()
  const { data: veiculos, isLoading, isError } = useQuery({
    queryKey: ['veiculos'],
    queryFn: veiculosApi.listar,
  })

  const closeModal = () => {
    setShowModal(false)
    setEditingVeiculo(undefined)
  }

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-slate-800">Veiculos</h1>
          <p className="text-slate-500 text-sm mt-1">Gerencie a frota de veiculos</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700"
        >
          <Plus size={16} />
          Novo Veiculo
        </button>
      </div>

      {isLoading && (
        <div className="flex items-center justify-center h-40">
          <Loader2 className="animate-spin text-blue-500" size={32} />
        </div>
      )}

      {isError && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700 text-sm">
          Erro ao carregar veiculos. Verifique se a API esta rodando.
        </div>
      )}

      {veiculos && (
        <VeiculosTable
          veiculos={veiculos}
          onEdit={(veiculo) => {
            setEditingVeiculo(veiculo)
            setShowModal(true)
          }}
        />
      )}

      {showModal && <VeiculoModal veiculo={editingVeiculo} onClose={closeModal} />}
    </div>
  )
}

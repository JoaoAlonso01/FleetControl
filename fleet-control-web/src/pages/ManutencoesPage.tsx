import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import type { AxiosError } from 'axios'
import { Plus, Wrench, Loader2, X } from 'lucide-react'
import { manutencoesApi } from '../services/manutencoesService'
import { veiculosApi } from '../services/veiculosService'
import type { ManutencaoResponse, StatusManutencao, AgendarManutencaoRequest } from '../types'

const schema = z.object({
  veiculoId: z.string().min(1, 'Veiculo obrigatorio'),
  tipo: z.string().min(1, 'Tipo obrigatorio'),
  descricao: z.string().min(1, 'Descricao obrigatoria'),
  dataAgendada: z
    .string()
    .min(1, 'Data obrigatoria')
    .refine(v => new Date(v).getTime() > Date.now(), 'Data deve ser no futuro'),
  oficina: z.string().optional(),
  custoEstimado: z.coerce.number().optional(),
})

type FormData = z.infer<typeof schema>
type ProblemDetails = { detail?: string; title?: string }

const statusColors: Record<StatusManutencao, string> = {
  Agendada: 'bg-blue-100 text-blue-800',
  EmAndamento: 'bg-yellow-100 text-yellow-800',
  Concluida: 'bg-green-100 text-green-800',
  Cancelada: 'bg-gray-100 text-gray-600',
}

const minDateTimeLocal = () => {
  const now = new Date()
  now.setMinutes(now.getMinutes() - now.getTimezoneOffset() + 1)
  return now.toISOString().slice(0, 16)
}

const getErrorMessage = (error: unknown) => {
  const axiosError = error as AxiosError<ProblemDetails>
  return axiosError.response?.data?.detail ?? axiosError.response?.data?.title ?? 'Erro ao agendar manutencao.'
}

function NovaManutencaoModal({ onClose }: { onClose: () => void }) {
  const qc = useQueryClient()
  const { data: veiculos } = useQuery({ queryKey: ['veiculos'], queryFn: veiculosApi.listar })
  const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema) as never,
  })

  const mutation = useMutation({
    mutationFn: (data: FormData) =>
      manutencoesApi.agendar({
        ...data,
        dataAgendada: new Date(data.dataAgendada).toISOString(),
        custoEstimado: data.custoEstimado || undefined,
      } as AgendarManutencaoRequest),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['manutencoes'] })
      qc.invalidateQueries({ queryKey: ['veiculos'] })
      onClose()
    },
  })

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6 max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-5">
          <h2 className="text-lg font-semibold text-slate-800">Agendar Manutencao</h2>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600" aria-label="Fechar">
            <X size={20} />
          </button>
        </div>
        <form onSubmit={handleSubmit(d => mutation.mutate(d))} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Veiculo</label>
            <select {...register('veiculoId')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">Selecione...</option>
              {veiculos?.filter(v => v.status !== 'Inativo').map(v => (
                <option key={v.id} value={v.id}>{v.placa} - {v.marca} {v.modelo}</option>
              ))}
            </select>
            {errors.veiculoId && <p className="text-red-500 text-xs mt-1">{errors.veiculoId.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Tipo</label>
            <select {...register('tipo')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">Selecione...</option>
              <option value="Preventiva">Preventiva</option>
              <option value="Corretiva">Corretiva</option>
              <option value="Revisao">Revisao</option>
            </select>
            {errors.tipo && <p className="text-red-500 text-xs mt-1">{errors.tipo.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Descricao</label>
            <textarea {...register('descricao')} rows={3} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            {errors.descricao && <p className="text-red-500 text-xs mt-1">{errors.descricao.message}</p>}
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Data Agendada</label>
              <input type="datetime-local" min={minDateTimeLocal()} {...register('dataAgendada')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              {errors.dataAgendada && <p className="text-red-500 text-xs mt-1">{errors.dataAgendada.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Custo Est. (R$)</label>
              <input type="number" step="0.01" {...register('custoEstimado')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Oficina</label>
            <input {...register('oficina')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>
          {mutation.isError && <p className="text-red-500 text-sm">{getErrorMessage(mutation.error)}</p>}
          <div className="flex gap-3 pt-2">
            <button type="button" onClick={onClose} className="flex-1 px-4 py-2 border border-slate-300 rounded-lg text-sm text-slate-700 hover:bg-slate-50">Cancelar</button>
            <button type="submit" disabled={mutation.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 flex items-center justify-center gap-2">
              {mutation.isPending && <Loader2 size={14} className="animate-spin" />}
              Agendar
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

function ManutencaoCard({ m }: { m: ManutencaoResponse }) {
  const qc = useQueryClient()
  const iniciar = useMutation({
    mutationFn: () => manutencoesApi.iniciar(m.id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['manutencoes'] }),
  })
  const cancelar = useMutation({
    mutationFn: () => manutencoesApi.cancelar(m.id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['manutencoes'] })
      qc.invalidateQueries({ queryKey: ['veiculos'] })
    },
  })

  return (
    <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-5">
      <div className="flex items-start justify-between mb-2">
        <div className="flex items-center gap-2">
          <Wrench className="text-yellow-500" size={18} />
          <span className="font-medium text-slate-800">{m.veiculoPlaca ?? m.veiculoId.slice(0, 8)}</span>
        </div>
        <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[m.status]}`}>
          {m.status}
        </span>
      </div>
      <p className="text-sm text-slate-600 mb-1">{m.tipo} - {m.descricao}</p>
      {m.oficina && <p className="text-xs text-slate-500">Oficina: {m.oficina}</p>}
      <p className="text-xs text-slate-500 mt-1">
        Agendada: {new Date(m.dataAgendada).toLocaleDateString('pt-BR')}
      </p>
      {m.custoEstimado && (
        <p className="text-xs text-slate-500">
          Custo est.: {m.custoEstimado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
        </p>
      )}
      {m.status === 'Agendada' && (
        <div className="flex gap-2 mt-3">
          <button onClick={() => iniciar.mutate()} className="text-xs px-3 py-1 bg-yellow-100 text-yellow-800 rounded-lg hover:bg-yellow-200">
            Iniciar
          </button>
          <button onClick={() => cancelar.mutate()} className="text-xs px-3 py-1 bg-red-50 text-red-600 rounded-lg hover:bg-red-100">
            Cancelar
          </button>
        </div>
      )}
    </div>
  )
}

export default function ManutencoesPage() {
  const [showModal, setShowModal] = useState(false)
  const { data: manutencoes, isLoading, isError } = useQuery({
    queryKey: ['manutencoes'],
    queryFn: manutencoesApi.listar,
  })

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-slate-800">Manutencoes</h1>
          <p className="text-slate-500 text-sm mt-1">Acompanhe as manutencoes da frota</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700"
        >
          <Plus size={16} />
          Agendar
        </button>
      </div>

      {isLoading && <div className="flex items-center justify-center h-40"><Loader2 className="animate-spin text-blue-500" size={32} /></div>}
      {isError && <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700 text-sm">Erro ao carregar manutencoes.</div>}

      {manutencoes && (
        <>
          {manutencoes.length === 0 ? (
            <div className="text-center py-16 text-slate-400">
              <Wrench size={48} className="mx-auto mb-3 opacity-40" />
              <p>Nenhuma manutencao registrada</p>
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              {manutencoes.map(m => <ManutencaoCard key={m.id} m={m} />)}
            </div>
          )}
        </>
      )}

      {showModal && <NovaManutencaoModal onClose={() => setShowModal(false)} />}
    </div>
  )
}

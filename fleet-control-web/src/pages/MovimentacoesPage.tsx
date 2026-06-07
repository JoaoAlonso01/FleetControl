import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { ArrowLeftRight, Loader2 } from 'lucide-react'
import { movimentacoesApi } from '../services/movimentacoesService'
import { veiculosApi } from '../services/veiculosService'
import { usuariosApi } from '../services/usuariosService'
import type { MovimentacaoResponse, RegistrarCheckOutRequest, RegistrarCheckInRequest } from '../types'

const checkOutSchema = z.object({
  veiculoId: z.string().min(1, 'Veículo obrigatório'),
  usuarioId: z.string().min(1, 'Usuário obrigatório'),
  quilometragemSaida: z.coerce.number().min(0),
  destino: z.string().optional(),
  observacao: z.string().optional(),
})

const checkInSchema = z.object({
  veiculoId: z.string().min(1, 'Veículo obrigatório'),
  usuarioId: z.string().min(1, 'Usuário obrigatório'),
  quilometragemRetorno: z.coerce.number().min(0),
  observacao: z.string().optional(),
})

type CheckOutForm = z.infer<typeof checkOutSchema>
type CheckInForm = z.infer<typeof checkInSchema>

function CheckOutForm() {
  const qc = useQueryClient()
  const { data: veiculos } = useQuery({ queryKey: ['veiculos'], queryFn: veiculosApi.listar })
  const { data: usuarios } = useQuery({ queryKey: ['usuarios'], queryFn: usuariosApi.listar })
  const { register, handleSubmit, reset, formState: { errors } } = useForm<CheckOutForm>({
    resolver: zodResolver(checkOutSchema) as never,
  })

  const mutation = useMutation({
    mutationFn: movimentacoesApi.checkOut,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['movimentacoes'] })
      qc.invalidateQueries({ queryKey: ['veiculos'] })
      reset()
    },
  })

  return (
    <form onSubmit={handleSubmit(d => mutation.mutate(d as unknown as RegistrarCheckOutRequest))} className="space-y-4">
      <h2 className="text-base font-semibold text-slate-800 flex items-center gap-2">
        <span className="w-2 h-2 rounded-full bg-red-500 inline-block"></span>
        Check-Out
      </h2>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Veículo</label>
        <select {...register('veiculoId')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option value="">Selecione...</option>
          {veiculos?.filter(v => v.status === 'Disponivel').map(v => (
            <option key={v.id} value={v.id}>{v.placa} — {v.marca} {v.modelo}</option>
          ))}
        </select>
        {errors.veiculoId && <p className="text-red-500 text-xs mt-1">{errors.veiculoId.message}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Motorista</label>
        <select {...register('usuarioId')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option value="">Selecione...</option>
          {usuarios?.filter(u => u.ativo && u.perfil === 'Motorista').map(u => (
            <option key={u.id} value={u.id}>{u.nome}</option>
          ))}
        </select>
        {errors.usuarioId && <p className="text-red-500 text-xs mt-1">{errors.usuarioId.message}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Quilometragem Saída</label>
        <input type="number" {...register('quilometragemSaida')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
        {errors.quilometragemSaida && <p className="text-red-500 text-xs mt-1">{errors.quilometragemSaida.message}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Destino</label>
        <input {...register('destino')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Observação</label>
        <textarea {...register('observacao')} rows={2} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
      </div>
      {mutation.isError && <p className="text-red-500 text-sm">Erro ao registrar check-out.</p>}
      {mutation.isSuccess && <p className="text-green-600 text-sm">Check-out registrado!</p>}
      <button type="submit" disabled={mutation.isPending} className="w-full px-4 py-2 bg-red-600 text-white rounded-lg text-sm font-medium hover:bg-red-700 disabled:opacity-50 flex items-center justify-center gap-2">
        {mutation.isPending && <Loader2 size={14} className="animate-spin" />}
        Registrar Check-Out
      </button>
    </form>
  )
}

function CheckInForm() {
  const qc = useQueryClient()
  const { data: veiculos } = useQuery({ queryKey: ['veiculos'], queryFn: veiculosApi.listar })
  const { data: usuarios } = useQuery({ queryKey: ['usuarios'], queryFn: usuariosApi.listar })
  const { register, handleSubmit, reset, formState: { errors } } = useForm<CheckInForm>({
    resolver: zodResolver(checkInSchema) as never,
  })

  const mutation = useMutation({
    mutationFn: movimentacoesApi.checkIn,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['movimentacoes'] })
      qc.invalidateQueries({ queryKey: ['veiculos'] })
      reset()
    },
  })

  return (
    <form onSubmit={handleSubmit(d => mutation.mutate(d as unknown as RegistrarCheckInRequest))} className="space-y-4">
      <h2 className="text-base font-semibold text-slate-800 flex items-center gap-2">
        <span className="w-2 h-2 rounded-full bg-green-500 inline-block"></span>
        Check-In
      </h2>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Veículo</label>
        <select {...register('veiculoId')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option value="">Selecione...</option>
          {veiculos?.filter(v => v.status === 'EmUso').map(v => (
            <option key={v.id} value={v.id}>{v.placa} — {v.marca} {v.modelo}</option>
          ))}
        </select>
        {errors.veiculoId && <p className="text-red-500 text-xs mt-1">{errors.veiculoId.message}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Motorista</label>
        <select {...register('usuarioId')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option value="">Selecione...</option>
          {usuarios?.filter(u => u.ativo).map(u => (
            <option key={u.id} value={u.id}>{u.nome}</option>
          ))}
        </select>
        {errors.usuarioId && <p className="text-red-500 text-xs mt-1">{errors.usuarioId.message}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Quilometragem Retorno</label>
        <input type="number" {...register('quilometragemRetorno')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
        {errors.quilometragemRetorno && <p className="text-red-500 text-xs mt-1">{errors.quilometragemRetorno.message}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">Observação</label>
        <textarea {...register('observacao')} rows={2} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
      </div>
      {mutation.isError && <p className="text-red-500 text-sm">Erro ao registrar check-in.</p>}
      {mutation.isSuccess && <p className="text-green-600 text-sm">Check-in registrado!</p>}
      <button type="submit" disabled={mutation.isPending} className="w-full px-4 py-2 bg-green-600 text-white rounded-lg text-sm font-medium hover:bg-green-700 disabled:opacity-50 flex items-center justify-center gap-2">
        {mutation.isPending && <Loader2 size={14} className="animate-spin" />}
        Registrar Check-In
      </button>
    </form>
  )
}

function MovimentacaoRow({ m }: { m: MovimentacaoResponse }) {
  return (
    <tr className="hover:bg-slate-50">
      <td className="px-6 py-3 text-sm text-slate-800">{m.veiculoPlaca ?? m.veiculoId.slice(0, 8)}</td>
      <td className="px-6 py-3 text-sm text-slate-600">{m.usuarioNome ?? m.usuarioId.slice(0, 8)}</td>
      <td className="px-6 py-3 text-sm text-slate-600">{m.quilometragemSaida.toLocaleString('pt-BR')} km</td>
      <td className="px-6 py-3 text-sm text-slate-600">
        {m.quilometragemRetorno ? `${m.quilometragemRetorno.toLocaleString('pt-BR')} km` : '—'}
      </td>
      <td className="px-6 py-3 text-sm text-slate-600">{m.destino ?? '—'}</td>
      <td className="px-6 py-3 text-sm text-slate-500">{new Date(m.dataCheckOut).toLocaleDateString('pt-BR')}</td>
      <td className="px-6 py-3">
        <span className={`px-2 py-1 rounded-full text-xs font-medium ${m.dataCheckIn ? 'bg-green-100 text-green-800' : 'bg-blue-100 text-blue-800'}`}>
          {m.dataCheckIn ? 'Concluída' : 'Em Curso'}
        </span>
      </td>
    </tr>
  )
}

export default function MovimentacoesPage() {
  const { data: movimentacoes, isLoading } = useQuery({
    queryKey: ['movimentacoes'],
    queryFn: movimentacoesApi.listar,
  })

  return (
    <div className="p-8">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-slate-800">Check-in / Check-out</h1>
        <p className="text-slate-500 text-sm mt-1">Controle de movimentação de veículos</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
          <CheckOutForm />
        </div>
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
          <CheckInForm />
        </div>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div className="px-6 py-4 border-b border-slate-200">
          <h2 className="text-base font-semibold text-slate-800 flex items-center gap-2">
            <ArrowLeftRight size={18} className="text-slate-500" />
            Histórico de Movimentações
          </h2>
        </div>
        {isLoading ? (
          <div className="flex items-center justify-center h-24"><Loader2 className="animate-spin text-blue-500" size={24} /></div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-slate-50 border-b border-slate-200">
                <tr>
                  {['Veículo', 'Motorista', 'Km Saída', 'Km Retorno', 'Destino', 'Data', 'Status'].map(h => (
                    <th key={h} className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {movimentacoes?.map(m => <MovimentacaoRow key={m.id} m={m} />)}
                {(!movimentacoes || movimentacoes.length === 0) && (
                  <tr>
                    <td colSpan={7} className="px-6 py-10 text-center text-slate-400 text-sm">
                      Nenhuma movimentação registrada
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}

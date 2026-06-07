import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Pencil, Plus, Users, Loader2, X, CheckCircle, XCircle } from 'lucide-react'
import { usuariosApi } from '../services/usuariosService'
import type { UsuarioResponse, PerfilUsuario, AtualizarUsuarioRequest } from '../types'

const schema = z.object({
  nome: z.string().min(1, 'Nome obrigatorio'),
  email: z.string().email('E-mail invalido'),
  perfil: z.string().min(1, 'Perfil obrigatorio'),
  telefone: z.string().optional(),
  numeroCnh: z.string().optional(),
})

const editSchema = z.object({
  nome: z.string().min(1, 'Nome obrigatorio'),
  perfil: z.string().min(1, 'Perfil obrigatorio'),
  telefone: z.string().optional(),
  numeroCnh: z.string().optional(),
})

type FormData = z.infer<typeof schema>
type EditFormData = z.infer<typeof editSchema>

const perfilLabels: Record<PerfilUsuario, string> = {
  Administrador: 'Administrador',
  Motorista: 'Motorista',
  Gestor: 'Gestor',
}

const perfilColors: Record<PerfilUsuario, string> = {
  Administrador: 'bg-purple-100 text-purple-800',
  Motorista: 'bg-blue-100 text-blue-800',
  Gestor: 'bg-orange-100 text-orange-800',
}

function NovoUsuarioModal({ onClose }: { onClose: () => void }) {
  const qc = useQueryClient()
  const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  const mutation = useMutation({
    mutationFn: usuariosApi.criar,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['usuarios'] })
      onClose()
    },
  })

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md p-6">
        <div className="flex items-center justify-between mb-5">
          <h2 className="text-lg font-semibold text-slate-800">Novo Usuario</h2>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600" aria-label="Fechar"><X size={20} /></button>
        </div>
        <form onSubmit={handleSubmit(d => mutation.mutate(d))} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Nome</label>
            <input {...register('nome')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            {errors.nome && <p className="text-red-500 text-xs mt-1">{errors.nome.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">E-mail</label>
            <input type="email" {...register('email')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Perfil</label>
            <select {...register('perfil')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">Selecione...</option>
              <option value="Administrador">Administrador</option>
              <option value="Motorista">Motorista</option>
              <option value="Gestor">Gestor</option>
            </select>
            {errors.perfil && <p className="text-red-500 text-xs mt-1">{errors.perfil.message}</p>}
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Telefone</label>
              <input {...register('telefone')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">CNH</label>
              <input {...register('numeroCnh')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>
          </div>
          {mutation.isError && <p className="text-red-500 text-sm">Erro ao cadastrar. Verifique os dados.</p>}
          <div className="flex gap-3 pt-2">
            <button type="button" onClick={onClose} className="flex-1 px-4 py-2 border border-slate-300 rounded-lg text-sm text-slate-700 hover:bg-slate-50">Cancelar</button>
            <button type="submit" disabled={mutation.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 flex items-center justify-center gap-2">
              {mutation.isPending && <Loader2 size={14} className="animate-spin" />}
              Cadastrar
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

function EditarUsuarioModal({
  usuario,
  onClose,
}: {
  usuario: UsuarioResponse
  onClose: () => void
}) {
  const qc = useQueryClient()
  const { register, handleSubmit, formState: { errors } } = useForm<EditFormData>({
    resolver: zodResolver(editSchema),
    defaultValues: {
      nome: usuario.nome,
      perfil: usuario.perfil,
      telefone: usuario.telefone ?? '',
      numeroCnh: usuario.numeroCnh ?? '',
    },
  })

  const mutation = useMutation({
    mutationFn: (data: EditFormData) =>
      usuariosApi.atualizar(usuario.id, data as AtualizarUsuarioRequest),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['usuarios'] })
      onClose()
    },
  })

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md p-6">
        <div className="flex items-center justify-between mb-5">
          <h2 className="text-lg font-semibold text-slate-800">Editar Usuario</h2>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600" aria-label="Fechar"><X size={20} /></button>
        </div>
        <form onSubmit={handleSubmit(d => mutation.mutate(d))} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Nome</label>
            <input {...register('nome')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            {errors.nome && <p className="text-red-500 text-xs mt-1">{errors.nome.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">E-mail</label>
            <input value={usuario.email} disabled className="w-full border border-slate-200 bg-slate-50 rounded-lg px-3 py-2 text-sm text-slate-500" />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Perfil</label>
            <select {...register('perfil')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="Administrador">Administrador</option>
              <option value="Motorista">Motorista</option>
              <option value="Gestor">Gestor</option>
            </select>
            {errors.perfil && <p className="text-red-500 text-xs mt-1">{errors.perfil.message}</p>}
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Telefone</label>
              <input {...register('telefone')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">CNH</label>
              <input {...register('numeroCnh')} className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>
          </div>
          {mutation.isError && <p className="text-red-500 text-sm">Erro ao salvar usuario.</p>}
          <div className="flex gap-3 pt-2">
            <button type="button" onClick={onClose} className="flex-1 px-4 py-2 border border-slate-300 rounded-lg text-sm text-slate-700 hover:bg-slate-50">Cancelar</button>
            <button type="submit" disabled={mutation.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 flex items-center justify-center gap-2">
              {mutation.isPending && <Loader2 size={14} className="animate-spin" />}
              Salvar
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default function UsuariosPage() {
  const [showModal, setShowModal] = useState(false)
  const [editingUsuario, setEditingUsuario] = useState<UsuarioResponse | undefined>()
  const { data: usuarios, isLoading, isError } = useQuery({
    queryKey: ['usuarios'],
    queryFn: usuariosApi.listar,
  })

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-slate-800">Usuarios</h1>
          <p className="text-slate-500 text-sm mt-1">Gerencie os usuarios do sistema</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700"
        >
          <Plus size={16} />
          Novo Usuario
        </button>
      </div>

      {isLoading && (
        <div className="flex items-center justify-center h-40">
          <Loader2 className="animate-spin text-blue-500" size={32} />
        </div>
      )}

      {isError && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700 text-sm">
          Erro ao carregar usuarios. Verifique se a API esta rodando.
        </div>
      )}

      {usuarios && (
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
          {usuarios.length === 0 ? (
            <div className="text-center py-16 text-slate-400">
              <Users size={48} className="mx-auto mb-3 opacity-40" />
              <p>Nenhum usuario cadastrado</p>
            </div>
          ) : (
            <table className="w-full">
              <thead className="bg-slate-50 border-b border-slate-200">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Nome</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">E-mail</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Perfil</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">CNH</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Status</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-slate-500 uppercase tracking-wider">Acoes</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {usuarios.map((u: UsuarioResponse) => (
                  <tr key={u.id} className="hover:bg-slate-50">
                    <td className="px-6 py-4 text-sm font-medium text-slate-800">{u.nome}</td>
                    <td className="px-6 py-4 text-sm text-slate-600">{u.email}</td>
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${perfilColors[u.perfil]}`}>
                        {perfilLabels[u.perfil]}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-600">{u.numeroCnh ?? '-'}</td>
                    <td className="px-6 py-4">
                      {u.ativo ? (
                        <span className="flex items-center gap-1 text-green-600 text-xs font-medium">
                          <CheckCircle size={14} /> Ativo
                        </span>
                      ) : (
                        <span className="flex items-center gap-1 text-slate-400 text-xs font-medium">
                          <XCircle size={14} /> Inativo
                        </span>
                      )}
                    </td>
                    <td className="px-6 py-4 text-right">
                      <button
                        onClick={() => setEditingUsuario(u)}
                        className="inline-flex items-center gap-2 text-xs px-3 py-1 rounded-lg border border-slate-200 text-slate-700 hover:bg-slate-50"
                      >
                        <Pencil size={13} />
                        Editar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      )}

      {showModal && <NovoUsuarioModal onClose={() => setShowModal(false)} />}
      {editingUsuario && (
        <EditarUsuarioModal usuario={editingUsuario} onClose={() => setEditingUsuario(undefined)} />
      )}
    </div>
  )
}

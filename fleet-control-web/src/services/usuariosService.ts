import api from '../lib/api'
import type {
  UsuarioResponse,
  CriarUsuarioRequest,
  AtualizarUsuarioRequest,
} from '../types'

export const usuariosApi = {
  listar: () => api.get<UsuarioResponse[]>('/api/v1/usuarios').then(r => r.data),
  obterPorId: (id: string) => api.get<UsuarioResponse>(`/api/v1/usuarios/${id}`).then(r => r.data),
  criar: (data: CriarUsuarioRequest) => api.post<UsuarioResponse>('/api/v1/usuarios', data).then(r => r.data),
  atualizar: (id: string, data: AtualizarUsuarioRequest) =>
    api.put<void>(`/api/v1/usuarios/${id}`, data).then(r => r.data),
  desativar: (id: string) => api.patch(`/api/v1/usuarios/${id}/desativar`).then(r => r.data),
  ativar: (id: string) => api.patch(`/api/v1/usuarios/${id}/ativar`).then(r => r.data),
}

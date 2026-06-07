import api from '../lib/api'
import type {
  ManutencaoResponse,
  AgendarManutencaoRequest,
  ConcluirManutencaoRequest,
} from '../types'

export const manutencoesApi = {
  listar: () => api.get<ManutencaoResponse[]>('/api/v1/manutencoes').then(r => r.data),
  obterPorId: (id: string) => api.get<ManutencaoResponse>(`/api/v1/manutencoes/${id}`).then(r => r.data),
  agendar: (data: AgendarManutencaoRequest) =>
    api.post<ManutencaoResponse>('/api/v1/manutencoes', data).then(r => r.data),
  iniciar: (id: string) => api.patch(`/api/v1/manutencoes/${id}/iniciar`).then(r => r.data),
  concluir: (id: string, data: ConcluirManutencaoRequest) =>
    api.patch(`/api/v1/manutencoes/${id}/concluir`, data).then(r => r.data),
  cancelar: (id: string) => api.delete(`/api/v1/manutencoes/${id}`).then(r => r.data),
}

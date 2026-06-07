import api from '../lib/api'
import type {
  MovimentacaoResponse,
  RegistrarCheckOutRequest,
  RegistrarCheckInRequest,
} from '../types'

export const movimentacoesApi = {
  listar: () => api.get<MovimentacaoResponse[]>('/api/v1/movimentacoes').then(r => r.data),
  obterPorId: (id: string) =>
    api.get<MovimentacaoResponse>(`/api/v1/movimentacoes/${id}`).then(r => r.data),
  checkOut: (data: RegistrarCheckOutRequest) =>
    api.post<MovimentacaoResponse>('/api/v1/movimentacoes/checkout', data).then(r => r.data),
  checkIn: (data: RegistrarCheckInRequest) =>
    api.post<MovimentacaoResponse>('/api/v1/movimentacoes/checkin', data).then(r => r.data),
}

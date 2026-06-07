import api from '../lib/api'
import type {
  VeiculoResponse,
  CriarVeiculoRequest,
  AtualizarVeiculoRequest,
  AtualizarKilometragemRequest,
} from '../types'

type ApiVeiculoResponse = Omit<VeiculoResponse, 'quilometragemAtual'> & {
  quilometragemAtual?: number
  kilometragemAtual?: number
}

const normalizarVeiculo = (veiculo: ApiVeiculoResponse): VeiculoResponse => ({
  ...veiculo,
  quilometragemAtual: veiculo.quilometragemAtual ?? veiculo.kilometragemAtual ?? 0,
})

export const veiculosApi = {
  listar: () => api.get<ApiVeiculoResponse[]>('/api/v1/veiculos').then(r => r.data.map(normalizarVeiculo)),
  obterPorId: (id: string) => api.get<ApiVeiculoResponse>(`/api/v1/veiculos/${id}`).then(r => normalizarVeiculo(r.data)),
  criar: (data: CriarVeiculoRequest) => api.post<ApiVeiculoResponse>('/api/v1/veiculos', data).then(r => normalizarVeiculo(r.data)),
  atualizar: (id: string, data: AtualizarVeiculoRequest) =>
    api.put<void>(`/api/v1/veiculos/${id}`, data).then(r => r.data),
  atualizarKilometragem: (id: string, data: AtualizarKilometragemRequest) =>
    api.patch<VeiculoResponse>(`/api/v1/veiculos/${id}/quilometragem`, data).then(r => r.data),
  enviarParaManutencao: (id: string) =>
    api.patch(`/api/v1/veiculos/${id}/manutencao/iniciar`).then(r => r.data),
  retornarDaManutencao: (id: string) =>
    api.patch(`/api/v1/veiculos/${id}/manutencao/retornar`).then(r => r.data),
  desativar: (id: string) => api.delete(`/api/v1/veiculos/${id}`).then(r => r.data),
}

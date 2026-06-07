export type StatusVeiculo = 'Disponivel' | 'EmUso' | 'EmManutencao' | 'Inativo'
export type PerfilUsuario = 'Administrador' | 'Motorista' | 'Gestor'
export type StatusManutencao = 'Agendada' | 'EmAndamento' | 'Concluida' | 'Cancelada'
export type TipoManutencao = 'Preventiva' | 'Corretiva' | 'Revisao'

export interface VeiculoResponse {
  id: string
  modelo: string
  marca: string
  ano: number
  placa: string
  status: StatusVeiculo
  quilometragemAtual: number
  criadoEm: string
  atualizadoEm: string
}

export interface CriarVeiculoRequest {
  modelo: string
  marca: string
  ano: number
  placa: string
}

export interface AtualizarVeiculoRequest {
  modelo: string
  marca: string
  ano: number
  placa: string
}

export interface AtualizarKilometragemRequest {
  quilometragem: number
}

export interface UsuarioResponse {
  id: string
  nome: string
  email: string
  perfil: PerfilUsuario
  telefone?: string
  numeroCnh?: string
  ativo: boolean
  criadoEm: string
  atualizadoEm: string
}

export interface CriarUsuarioRequest {
  nome: string
  email: string
  perfil: string
  telefone?: string
  numeroCnh?: string
}

export interface AtualizarUsuarioRequest {
  nome: string
  perfil: string
  telefone?: string
  numeroCnh?: string
}

export interface ManutencaoResponse {
  id: string
  veiculoId: string
  veiculoPlaca?: string
  tipo: TipoManutencao
  descricao: string
  dataAgendada: string
  dataInicio?: string
  dataConclusao?: string
  oficina?: string
  custoEstimado?: number
  custoReal?: number
  status: StatusManutencao
  criadoEm: string
}

export interface AgendarManutencaoRequest {
  veiculoId: string
  tipo: string
  descricao: string
  dataAgendada: string
  oficina?: string
  custoEstimado?: number
}

export interface ConcluirManutencaoRequest {
  custoReal: number
}

export interface MovimentacaoResponse {
  id: string
  veiculoId: string
  veiculoPlaca?: string
  usuarioId: string
  usuarioNome?: string
  quilometragemSaida: number
  quilometragemRetorno?: number
  destino?: string
  observacao?: string
  dataCheckOut: string
  dataCheckIn?: string
  tipo: string
  criadoEm: string
}

export interface RegistrarCheckOutRequest {
  veiculoId: string
  usuarioId: string
  quilometragemSaida: number
  destino?: string
  observacao?: string
}

export interface RegistrarCheckInRequest {
  veiculoId: string
  usuarioId: string
  quilometragemRetorno: number
  observacao?: string
}

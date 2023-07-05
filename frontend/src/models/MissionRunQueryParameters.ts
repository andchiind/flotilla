import { MissionStatus } from './Mission'

export interface MissionRunQueryParameters {
    status?: MissionStatus
    robotId?: string
    nameSearch?: string
    robotNameSearch?: string
    tagSearch?: string
    area?: string
    minStartTime?: number
    maxStartTime?: number
    minEndTime?: number
    maxEndTime?: number
    minDesiredStartTime?: number
    maxDesiredStartTime?: number
    pageNumber?: number
    pageSize?: number
    orderBy?: string
}
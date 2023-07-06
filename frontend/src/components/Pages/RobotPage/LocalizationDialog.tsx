import { Autocomplete, AutocompleteChanges, Button, Card, Dialog, Typography, Icon } from '@equinor/eds-core-react'
import styled from 'styled-components'
import { TranslateText } from 'components/Contexts/LanguageContext'
import { Icons } from 'utils/icons'
import { useState, useEffect } from 'react'
import { BackendAPICaller } from 'api/ApiCaller'
import { Area } from 'models/Area'
import { Robot } from 'models/Robot'
import { AreaMapView } from './AreaMapView'

const StyledDialog = styled(Card)`
    display: flex;
    padding: 1rem;
    width: 600px;
    right: 175px;
`

const StyledAutoComplete = styled.div`
    display: flex;
    flex-direction: row;
    justify-content: space-evenly;
`

const StyledLocalizationButton = styled.div`
    display: flex;
`

const StyledButtons = styled.div`
    display: flex;
    gap: 8px;
    justify-content: flex-end;
`

interface RobotProps {
    robot: Robot
}

export const LocalizationDialog = ({ robot }: RobotProps): JSX.Element => {
    const [isLocalizationDialogOpen, setIsLocalizationDialogOpen] = useState<boolean>(false)
    const [selectedArea, setSelectedArea] = useState<Area>()
    const [areas, setAreas] = useState<Area[]>()

    useEffect(() => {
        BackendAPICaller.getAreas().then((response: Area[]) => {
            setAreas(response)
        })
    }, [])

    const getAreaNames = (areas: Area[]): Map<string, Area> => {
        var areaNameMap = new Map<string, Area>()
        areas.map((area: Area) => {
            areaNameMap.set(area.deckName, area)
        })
        return areaNameMap
    }

    const onSelectedDeck = (changes: AutocompleteChanges<string>) => {
        const selectedDeckName = changes.selectedItems[0]
        const selectedArea = areas?.find((area) => area.deckName === selectedDeckName)
        setSelectedArea(selectedArea)
    }

    const onClickLocalizeRobot = () => {
        setIsLocalizationDialogOpen(true)
    }

    const onLocalizationDialogClose = () => {
        setIsLocalizationDialogOpen(false)
        setSelectedArea(undefined)
    }

    const onClickLocalize = () => {
        if (selectedArea) {
            BackendAPICaller.postLocalizationMission(selectedArea?.defaultLocalizationPose, robot.id, selectedArea.id)
        }
        onLocalizationDialogClose()
    }

    const areaNames = areas ? Array.from(getAreaNames(areas).keys()).sort() : []

    return (
        <>
            <StyledLocalizationButton>
                <Button
                    onClick={() => {
                        onClickLocalizeRobot()
                    }}
                >
                    <>
                        <Icon name={Icons.PinDrop} size={16} />
                        {TranslateText('Localize robot')}
                    </>
                </Button>
            </StyledLocalizationButton>
            <Dialog open={isLocalizationDialogOpen} isDismissable>
                <StyledDialog>
                    <Typography variant="h2">{TranslateText('Localize robot')}</Typography>
                    <StyledAutoComplete>
                        <Autocomplete
                            options={areaNames}
                            label={TranslateText('Select deck')}
                            onOptionsChange={onSelectedDeck}
                        />
                    </StyledAutoComplete>
                    {selectedArea && <AreaMapView area={selectedArea} />}
                    <StyledButtons>
                        <Button
                            onClick={() => {
                                onLocalizationDialogClose()
                            }}
                            variant="outlined"
                            color="secondary"
                        >
                            {' '}
                            {TranslateText('Cancel')}{' '}
                        </Button>
                        <Button onClick={onClickLocalize} disabled={!selectedArea}>
                            {' '}
                            {TranslateText('Localize')}{' '}
                        </Button>
                    </StyledButtons>
                </StyledDialog>
            </Dialog>
        </>
    )
}

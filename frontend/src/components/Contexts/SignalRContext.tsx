import * as signalR from '@microsoft/signalr'
import { useIsAuthenticated } from '@azure/msal-react'
import { createContext, FC, useContext, useEffect, useState } from 'react'
import { AuthContext } from './AuthProvider'
import { config } from 'config'

/**
 * SignalR provides asynchronous communication between backend and frontend. This
 * context provides functions for establishing event listeners and for accessing
 * the connection object, primarily to verify that a connection has been made.
 *
 * When registering an event handler using "registerEvent" an event name needs to be
 * provided, which must correspond to the event name used on the backend. The event
 * handler should receive a username and a message, though the username is typically
 * not relevant for broadcasted messages.
 *
 * It is important to note that event handlers can only see the scope at which they
 * are defined, which means that any React state will be outdated once they receive
 * a message. It is therefore important to update React state within these handlers
 * by passing functions in the setter functions. For instance instead of doing:
 *
 *   setState({...state, newEntry})
 *
 * we must instead do:
 *
 *   setState((oldState) => { return {...oldState, newEntry} })
 *
 * When accessing this context within another context, it is also important to make
 * sure that the other context provider is nested within the signalR context
 * provider.
 *
 * Objects are received as JSON strings. When parsing these it is important to note
 * that enums are typically encoded as numbers on the backend, and may therefore
 * need to be translated to string enums when making comparisons on the frontend.
 */

interface ISignalRContext {
    connection: signalR.HubConnection | undefined
    registerEvent: (eventName: string, onMessageReceived: (username: string, message: string) => void) => void
    connectionReady: boolean
}

interface Props {
    children: React.ReactNode
}

const defaultSignalRInterface = {
    connection: undefined,
    registerEvent: (eventName: string, onMessageReceived: (username: string, message: string) => void) => {},
    connectionReady: false,
}

const URL = config.BACKEND_API_SIGNALR_URL

export const SignalRContext = createContext<ISignalRContext>(defaultSignalRInterface)

export const SignalRProvider: FC<Props> = ({ children }) => {
    const [connection, setConnection] = useState<signalR.HubConnection | undefined>(defaultSignalRInterface.connection)
    const [connectionReady, setConnectionReady] = useState<boolean>(defaultSignalRInterface.connectionReady)
    const isAuthenticated = useIsAuthenticated()
    const accessToken = useContext(AuthContext)

    useEffect(() => {
        if (isAuthenticated && accessToken) {
            var newConnection = new signalR.HubConnectionBuilder()
                .withUrl(URL, {
                    accessTokenFactory: () => accessToken,
                    transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
                })
                .withAutomaticReconnect()
                .build()

            newConnection
                .start()
                .then(() => {
                    console.log('SignalR connection made: ', newConnection)
                    setConnection(newConnection)
                    setConnectionReady(true)
                })
                .catch((err) => document.write(err))
        }
    }, [isAuthenticated, accessToken])

    const registerEvent = (eventName: string, onMessageReceived: (username: string, message: string) => void) => {
        if (connection) {
            connection.on(eventName, (username, message) => {
                onMessageReceived(username, message)
            })
        }
    }

    return (
        <SignalRContext.Provider
            value={{
                connection,
                registerEvent,
                connectionReady,
            }}
        >
            {children}
        </SignalRContext.Provider>
    )
}

export const useSignalRContext = () => useContext(SignalRContext)
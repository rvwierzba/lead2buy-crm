import * as signalR from "@microsoft/signalr";
import { useAuthStore } from "@/stores/authStore";

const URL = "http://localhost:5000/notificationHub"; // A URL do nosso Hub

class SignalRService {
    constructor() {
        this.connection = null;
    }

    startConnection() {
        const authStore = useAuthStore();
        const token = authStore.token;

        if (!token) {
            console.log("SignalR: Nenhum token encontrado, conexão não iniciada.");
            return;
        }

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(URL, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        this.connection.start()
            .then(() => console.log("SignalR Conectado!"))
            .catch(err => console.error("Erro na conexão SignalR: ", err));
    }

    // Método para "ouvir" eventos do servidor
    on(eventName, callback) {
        this.connection?.on(eventName, callback);
    }

    // Método para parar a conexão (ex: no logout)
    stopConnection() {
        this.connection?.stop()
            .then(() => console.log("SignalR Desconectado."));
    }
}

export default new SignalRService();

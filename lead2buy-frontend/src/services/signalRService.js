import * as signalR from "@microsoft/signalr";
import { useAuthStore } from '@/stores/authStore';

class SignalRService {
    constructor() {
        this.connection = null;
        this.startPromise = null;
    }

    startConnection() {
        const authStore = useAuthStore();
        const token = authStore.token;

        if (!token) {
            console.log("SignalR: Token não encontrado. Conexão não iniciada.");
            return Promise.reject("Token não encontrado.");
        }

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        this.startPromise = this.connection.start()
            .then(() => console.log("SignalR Connection Started"))
            .catch(err => {
                console.error("SignalR Connection Error: ", err);
                // Limpa a promessa em caso de falha para permitir nova tentativa
                this.startPromise = null;
                return Promise.reject(err);
            });

        return this.startPromise;
    }

    // Garante que a conexão está pronta antes de registrar um listener
    async on(methodName, newMethod) {
        if (!this.startPromise) {
            // Se a conexão não foi nem tentada, inicia agora
            await this.startConnection();
        } else {
            // Se já foi tentada, apenas aguarda a conclusão
            await this.startPromise;
        }

        if (this.connection) {
            this.connection.on(methodName, newMethod);
        }
    }

    stopConnection() {
        if (this.connection) {
            this.connection.stop();
            this.connection = null;
            this.startPromise = null;
        }
    }
}

export default new SignalRService();

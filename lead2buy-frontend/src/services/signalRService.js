import * as signalR from "@microsoft/signalr";
import { useAuthStore } from "@/stores/authStore";

const signalRService = {
  connection: null,

  startConnection() {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      console.log("A conexão SignalR já está ativa.");
      return;
    }

    const authStore = useAuthStore();

    // --- A CORREÇÃO FINAL ESTÁ AQUI ---
    // Usa um caminho relativo para se conectar ao mesmo domínio (https://crm.rvwtech.com.br)
    const hubUrl = "/notificationHub";
    // --- FIM DA CORREÇÃO ---

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => authStore.token,
      })
      .withAutomaticReconnect()
      .build();

    this.connection.start()
      .then(() => console.log("Conexão SignalR estabelecida com sucesso."))
      .catch(err => {
        console.error("Erro na conexão SignalR: ", err);
        // Tenta reconectar após um atraso
        setTimeout(() => this.startConnection(), 5000);
      });

    this.connection.onclose(async () => {
      console.log("Conexão SignalR fechada. Tentando reconectar...");
      await this.startConnection();
    });
  },

  registerJobStatusUpdate(callback) {
    if (this.connection) {
      this.connection.on("JobStatusUpdate", callback);
    }
  },

  stopConnection() {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }
  }
};

export default signalRService;

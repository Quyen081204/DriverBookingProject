import * as signalR from "@microsoft/signalr";
import { host } from "../../data/api/clients";
import AsyncStorage from "@react-native-async-storage/async-storage";

export class SignalRClient {
  private static connectionInstance: signalR.HubConnection | null = null;

  public static async getConnectionInstanceAsync(): Promise<signalR.HubConnection> {
    if (!this.connectionInstance) {
      const token = await AsyncStorage.getItem("token");

      this.connectionInstance = new signalR.HubConnectionBuilder()
        .withUrl(`${host}/bookingHub`, {
          accessTokenFactory: () => token ?? "",
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      this.connectionInstance.on("testReceiveMsg", (serverMsg: string) => {
        console.log(serverMsg);
      });

      await this.connectionInstance.start();
      await this.connectionInstance.send(
        "TestConnectedBySendMessage",
        "This from Quyen app ",
        "Server can hear that ?"
      );
    }

    return this.connectionInstance;
  }

  public static async stopConnectionAsync() {
    if (this.connectionInstance) {
      await this.connectionInstance.stop();
      this.connectionInstance = null;
    }
  }
}



// export async function startConnection() {
//     try {
//         await connection.start();
//         console.log("SignalR Connected.");
//     } catch (err) {
//         console.log(err);
//         setTimeout(startConnection, 5000);
//     }
// };


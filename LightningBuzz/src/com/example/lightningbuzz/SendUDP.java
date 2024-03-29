package com.example.lightningbuzz;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

public class SendUDP implements Runnable{
		String Command="";
		String UserNm="";
		String PKey="";
		String ServerIp="";
		String PortAddr="";
		String Ans="";
		
		@Override
		public void run() {
			  String udpMsg = Command + "|" + UserNm + "|" + PKey + "|" + Ans;
			    DatagramSocket ds = null;
			    try {
			        InetAddress serverAddr = InetAddress.getByName(ServerIp);
			        DatagramPacket dp;
			        dp = new DatagramPacket(udpMsg.getBytes(), udpMsg.length(), serverAddr, Integer.parseInt(PortAddr));
			        ds = new DatagramSocket();
			        ds.send(dp);
			    } catch (SocketException e) {
			        e.printStackTrace();
			    }catch (UnknownHostException e) {
			        e.printStackTrace();
			    } catch (IOException e) {
			        e.printStackTrace();
			    } catch (Exception e) {
			        e.printStackTrace();
			    } finally {
			        if (ds != null) {
			            ds.close();
			        }}
		}
}

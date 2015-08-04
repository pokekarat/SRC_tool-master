import java.net.*;
import java.io.*;
import java.util.*;
import java.text.*;

public class server
{
	public static void main(String[] args)
	{
		
		try{
			
			ServerSocket ss = null;
			ss = new ServerSocket(9898);
			
			System.out.println("Waiting client to connect...");
			Socket s = ss.accept();
			
			BufferedReader in = new BufferedReader(new InputStreamReader(s.getInputStream()));
			PrintWriter out = new PrintWriter(s.getOutputStream(), true);
			
			String incomingMsg;
			
			Calendar cal = Calendar.getInstance();
			long prev = cal.getTimeInMillis();
			long curr = prev;
			
			while((incomingMsg = in.readLine()) != null)
			{
				System.out.println("Get " + incomingMsg + " " + (curr - prev) + "\n");
				out.println(incomingMsg);
			}
						
			in.close();
			s.close();
		}
		catch(Exception e)
		{
		}
	}
}
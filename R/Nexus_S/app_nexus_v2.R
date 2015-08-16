source("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\Nexus\\nexus_power_function.r")
arg1 <- "lm"
mae_avg <- c()
appName <- c("Chrome(3g)") #c("Angrybird","GpuBench","Google map","Firefox(web)","Chrome(web)","Chrome(youtube)","Firefox(youtube)")

for(j in c(8:8))
{	
  errName <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\Nexus\\Real_Test\\app",j,"\\",arg1,"\\error.txt", sep="")
  sink(errName)
  
	sum <- c()
	sum2 <- c()
	
	for(i in c(1:7))
	{
		fileName <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\Nexus\\Real_Test\\app",j,"\\raw_data_",i,".txt", sep="")
		
		if(!file.exists(fileName)) next
		
		app <- read.table(fileName, header=TRUE, sep= " ")
		
		
		
		#print(app[1,8:20])
		
		if(arg1 == "lm")
		{
			cpu_pw <- cpu(app)
			screen_pw <- screen(app)
			#wifi_pw <- wifi(app)
			gpu_pw <- gpu(app)
			tg_pw <- tg(app)
		}
		
	
		if(arg1 == "src")
		{
			cpu_pw <- cpu_src(app)
			screen_pw <- screen_src(app)
			#wifi_pw <- wifi(app)
			gpu_pw <- gpu_src(app) 
		  tg_pw <- tg_src(app)
		}
		
		est_pw <- cpu_pw + screen_pw + gpu_pw + tg_pw
		
		#calculate 3G power wrt #packets
		mea_pw <- app$power
		"tg_pw <- mea_pw - est_pw
		tgb <- cbind(app$tx+app$rx,tg_pw)
		row.names(tgb) <- NULL
    "
		#write.table(tgb,"C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\Nexus\\Real_Test\\app9\\tgb2.txt", sep=" ", col.names = NA)
	
		acc_src <- accuracy(mea_pw,est_pw)
		
		cat("\nMAPE src = ")
		cat(acc_src[5])
	
		mae_avg <- append(mae_avg, acc_src[5])
		
		pngName <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\nexus\\Real_Test\\app",j,"\\",arg1,"\\app",i,".png", sep="")
		png(pngName, width=800, height=450, units ="px", res = 200)
		par(mar= c(2.5, 3.0, 1.0, 1.0), mgp=c(1.2,0.5,0), xaxs="r", yaxs= "r", cex.axis = 0.6,cex.lab  = 0.8)
		
		
		if(FALSE)
		{
			plot(mea_pw,type="l",col="red", xlab="Time(sec)", ylab="Power(mW)", ylim=c(0,3000))
			lines(est_pw,type="l",col="blue")	
			lines(cpu_pw,type="l",col="green")
			lines(screen_pw+cpu_pw, type="l",col="purple")
			lines(gpu_pw+screen_pw+cpu_pw, type="l", col="orange") #,lty=3,lwd=3)
			lines(gpu_pw+screen_pw+cpu_pw+wifi_pw, type="l", col="pink")
			
			#legend("topright", legend=c("Measure", "Estimate"), fill=c("red", "blue"), density=c(20, NA), bty="n")
			legend("topright", c("Measure","cpu","scr","gpu","wifi"), lty=c(1,1,1,1,1), lwd=c(2.5,2.5,2.5,2.5,2.5),
			#col=c("red","blue","green","purple","orange","pink"))
			col = c("red","green","purple","orange","pink"))
			title(appName[j])
		}
		else 
		{
			plot(mea_pw, type="l", xlab="Time(sec)", ylab="Power(mW)", ylim=c(0,8000), lwd=2)
			lines(est_pw, type="l", lty=1, lwd=1.2)
			legend("topright", c("Measured power","Estimated power"), lty=c(1,2), lwd=c(2,1.2),pt.cex=0.2, cex=0.5)
			dev.off()
		}
		
	}
	
	sdata <- sort(mae_avg)
	cat("\nmae_avg = ")
	cat(mean(sdata[2]:sdata[length(sdata)-1]))
	
	sink()
}




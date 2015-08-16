source("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\s4_power_function.r")
ind <- 1
arg1 <- "firefox_web_3g_lm"
appName <- c("Ftp")
errName <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\3G\\data_081315\\",arg1,"\\error",ind,".txt", sep="")
sink(errName)

tableName <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\3G\\data_081315\\",arg1,"\\raw_data_",ind,".txt", sep="")
app <- read.table(tableName, header=TRUE, sep= "")

#filter any data that too high and low
#app3 <- apply(app[,which(colnames(app)=="tx"): which(colnames(app)=="vps")], 2, remove_outliers)
#for(i in 1:22){ 	
#  app[,i+34] <- app3[,i]
#}
app[is.na(app)] <- 1 

#cpu
cpu_src_pw <- cpu_lm(app)

#screen
bright_src_pw <- bright_lm(app)

#gpu
gpu_src_pw <- gpu_src(app)

#3G
threeg_src_pw <- threeg_lm(app)

#estimated
est_src_pw <- cpu_src_pw + gpu_src_pw + bright_src_pw + threeg_src_pw #+ wifi_src_pw + screen_src_pw
#write.table(est_src_pw, "C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\3G\\data\\chrome_web_3g\\src_pw.txt", col.names=F, row.names=F)

mea_pw <- app$power
#mea_pw <- mea_pw * 1

xlen <- length(mea_pw)
acc_src <- accuracy(mea_pw, est_src_pw)

#cat("MAPE lm = ")
#cat(acc_lm[5])
#cat(" MAE lm= ")
#cat(acc_lm[3])
#cat("\n")

cat("MAPE src = ")
cat(acc_src[5])
cat(" MAE src= ")
cat(acc_src[3])
cat("\n")


pngName <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\3G\\data_081315\\",arg1,"\\app_",ind,".png", sep="")
png(pngName, width=800, height=450, units ="px", res = 200)
par(mar= c(2.5, 3.0, 1.0, 1.0), mgp=c(1.2,0.5,0), xaxs="r", yaxs= "r", cex.axis = 0.6,cex.lab  = 0.8)

if(FALSE){
	xlen <- length(mea_pw)
	plot(c(1:(xlen-2)),y=mea_pw[3:xlen],type="l",col="red", xlab="Time(sec)", ylab="Power(mW)", ylim=c(0,8000), lwd=2.5)
	lines(c(1:(xlen-2)),y=est_src_pw[1:(xlen-2)],type="l",col="blue")	
	legend("topright", c("Measured","Estimated"), lty=c(1,1), lwd=c(2.5,2.5),
	#col=c("red","blue","green","purple","orange","pink"))
	col = c("red","blue"))
	title(appName[j])
}

if(TRUE)
{
	plot(mea_pw,type="l", xlab="Time(sec)", ylab="Power(mW)", ylim=c(0,8000), lwd=2)
	lines(est_src_pw,type="l",lty=1, lwd=1.2)	
	lines(threeg_src_pw,type="o",lty=1, lwd=0.5)
	#lines(cpu_src_pw+threeg_src_pw, type="o", lty=1, lwd=0.8)
	#lines(est_pw,type="l",lty=2, lwd=1.2)	
	legend("topright", c("Measured power","Estimated power (SRC)", "Estimated power (Mixed models)"), lty=c(1,1,2), lwd=c(2,1.2,1.2),pt.cex=0.2, cex=0.5)
}

dev.off()

sink()


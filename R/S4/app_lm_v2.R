source("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\s4_power_function.r")
arg1 <- "lm"
src <- "chrome_web_3g"
target <- "chrome_web_3g\\lm"
data <- c()

errName <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\3G\\data_081315\\",target,"\\error.txt", sep="")
sink(errName)

mae_avg <- c()

for(ind in c(1:7))
{
  
      raw_data <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\3G\\data_081315\\",src,"\\raw_data_",ind,".txt", sep="")
      app <- read.table(raw_data, header=TRUE, sep= "")
    
      app[is.na(app)] <- 1 
      
      
      if(arg1 == "src")
      {
        #cpu
        cpu_pw <- cpu_lm(app)
        
        #screen
        bright_pw <- bright_src(app)
        
        #gpu
        #train_gpu <- read.table("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\GPU\\3\\gpu_power_3.txt", header=TRUE,sep= " ")
        #dataLen <- length(train_gpu[,1])
        #model.gpu <- svm(power ~ ., data = train_gpu, scale = FALSE )
        #gpu_df <- data.frame(app[42:60])
        #gpu_pw <- predict(model.gpu, gpu_df)
        gpu_pw <- gpu_src(app)
        
        #3G
        threeg_pw <- threeg_src(app)
      }
      
      if(arg1 == "lm"){
        
        #cpu
        cpu_pw <- cpu_lm(app)
        
        #screen
        bright_pw <- bright_lm(app)
        
        #gpu
        train_gpu <- read.table("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\GPU\\3\\gpu_power_3.txt", header=TRUE,sep= " ")
        dataLen <- length(train_gpu[,1])
        model.gpu <- svm(power ~ ., data = train_gpu, scale = FALSE )
        gpu_df <- data.frame(app[42:60])
        gpu_pw <- predict(model.gpu, gpu_df)
        
        #3G
        threeg_pw <- threeg_lm(app)
        
        
      }
      
     
      
      #estimated
      est_pw <- cpu_pw + gpu_pw + bright_pw + threeg_pw #+ wifi_src_pw + screen_src_pw
      mea_pw <- app$power
      

      acc_src <- accuracy(mea_pw, est_pw)
      #cat("MAPE = ")
      #cat(acc_src[5])
      
      #cat(" MAE = ")
      #cat(acc_src[3])
      
      cat("\n")
      
      #mae_avg <- mae_avg + acc_src[5]
      mae_avg <- append(mae_avg, acc_src[5])
      
      pngName <- paste("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\3G\\data_081315\\",target,"\\app_",ind,".png", sep="")
      png(pngName, width=800, height=450, units ="px", res = 200)
      par(mar= c(2.5, 3.0, 1.0, 1.0), mgp=c(1.2,0.5,0), xaxs="r", yaxs= "r", cex.axis = 0.6,cex.lab  = 0.8)
      
      plot(mea_pw,type="l", xlab="Time(sec)", ylab="Power(mW)", ylim=c(0,8000), lwd=2)
      lines(est_pw,type="l",lty=1, lwd=1.2)
      legend("topright", c("Measured power","Estimated power"), lty=c(1,2), lwd=c(2,1.2),pt.cex=0.2, cex=0.5)
      
      dev.off()
     
}
#browser()
sdata <- sort(mae_avg)
cat("mae_avg")
cat(mean(sdata[2]:sdata[length(sdata)-1]))
sink()


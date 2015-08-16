if(!is.element("apcluster", installed.packages()[,1]) )
{
	local({r <- getOption("repos")
		r["CRAN"] <- "http://cran.csie.ntu.edu.tw/"
		options(repos=r)})
	install.packages("apcluster")

}else{ 
	print("apcluster already installed.")
}

if(!is.element("kernlab", installed.packages()[,1]) )
{
	local({r <- getOption("repos")
		r["CRAN"] <- "http://cran.csie.ntu.edu.tw/"
		options(repos=r)})
	install.packages("kernlab")

}else{ 
	print("kernlab already installed.")
}

if(!is.element("e1071", installed.packages()[,1]) )
{
	local({r <- getOption("repos")
		r["CRAN"] <- "http://cran.csie.ntu.edu.tw/"
		options(repos=r)})
	install.packages("e1071")

}else{ 
	print("e1071 already installed.")
}
require(apcluster)
require(Rcpp)
library("e1071")
library(forecast)
library(MASS)

cpu <- function(app){
	
	util <- app$util0
	freq <- app$freq0
	ist <- app$it0s0
	ise <- app$iu0s0 

	power <- -1
	
	#ifelse(freq < 10000, freq <- freq * 1000, freq)
	ifelse(freq > 10000, freq <- freq / 1000, freq)
	
	if(TRUE)
	{
		power <- (-0.219 * (ist/(ise+1))) + (3.331 * util) + (0.366 * freq) + 191.089 
		#cat(power)
		
	}
	
	#if(FALSE){
	#ifelse(freq == 200000, power <- (-0.1255 * (ist/ise)) + (0.7225 * util)+ 389.2394,
	#		ifelse(freq == 400000, power <- (-0.2373 * (ist/ise)) + (1.6019 * util)+ 427.27,
	#				ifelse(freq == 800000, power <- (-0.7429 * (ist/ise)) + (4.0937 * util)+ 444.2811,
	#						ifelse(freq == 1000000, power <- (-1.131 * (ist/ise)) + (6.047 * util)+ 468.957, -1)
	#							)
	#				)
	#		)
	#}
	
	#print(power)
	return(power)
}

gpu <- function(app){
  #browser()
  train_gpu <- read.table("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\Nexus\\gpu\\train_less.txt", header=TRUE, sep= "\t")
  dataLen <- length(train_gpu[,1])
  model.gpu <- svm(power ~ ., data = train_gpu)
  col1 <- which(colnames(app)=="ftime")
  col2 <- which(colnames(app)=="vpf")
  
  gpu_df <- data.frame(app[col1:col2])
  power <- predict(model.gpu, gpu_df)
  return(power)
}

screen <- function(app)
{
	bright <- app$bright
	power <-(2.207 * bright) - 23.723
	return(power)
}

tg <- function(app){
  
  tx <- app$tx
  rx <- app$rx
  x <- tx + rx
  
  #power <- 20.47*x + 636.39
  #power =  12.68*(x) + 690.31
  power = 1.63 * x + 622
  return(power)
  
}

wifi <- function(app){

	tx <- app$tx
	rx <- app$rx
	ch <- 54
	
	if(is.null(app$status)){
		power <- 0
	}else{
		ifelse(app$status == "up",	power <- 122.80215 +  (-0.08698 * (tx+rx)),	power <- 0)
	}
	
	#ifelse(status == 1.000,power <- (-2.097 * ch) + (1.355 * (tx+rx)) + 51.970, ifelse(status == 0,power <- 51.97,power <- 0))
	#power <- (-2.097 * ch) + (1.355 * (tx+rx)) + 51.970
	
	power[power<0]<-0
	
	
	
	return(power)
}

cpu_src <- function(app){
	util <- app$util0
	freq <- app$freq0
	ist <- app$it0s0
	ise <- app$iu0s0
	power <- -1
	
	ifelse(freq > 10000, freq <- freq / 1000, freq)
	
	power = 268.26 + (2.12*util) + (0.34*freq) + (-0.46 * (ist/(1 + ise)))
	
	return(power)
}

screen_src <- function(app){
	
	bright <- app$bright
	power = 2.189*bright - 22.34
	return(power)
 
 }

asyncTable <- read.table("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\Nexus\\gpu\\asyncTable.txt", header=TRUE, sep= "\t")
totalAsync <- sum(asyncTable$power)
useAsync <- 0
numCol <- length(asyncTable[1,])
numLine <- length(asyncTable[,1])

gpu_src <- function(app)
{
	#browser()
	#power = 46.7509107971998 + 10625.2410997935*(app$txt_uld)/(2.70817721934747 + (app$txt_uld)*(app$vpf))
	
	txt_uld <- app$txt_uld
	usse_load_pix <- app$usse_load_pix
	ta_load <- app$ta_load
	vpf <- app$vpf
	fps <- app$fps
	g3d_core <- app$gtl3d_core
	gta_core <- app$gtlta_core
	usse_load_ver <- app$usse_load_ver
	
	#cat("power =",power,"\n")
	#power = (14553.2229860592*app$usse_load_ver) + 
	#		(31231.9174876562/app$vpf) + 
	#		(0.582053784806286/cos(5.33071558988574 + 26920.2063131213*app$usse_load_ver)) - 
	#		68.5263888732485 - 
	#		(7.7953930691623*app$fps) - 
	#		(3484.27008129347*app$ftime)
	#power = 384.796544649411 + (17116.5136399577 - (382.447061321096*app$vpf) - (22601.4226708935*app$ftime))/app$fps
	
	#good
	power = 540.95 - 3.44 * app$vpf
	#power = 140.996749035324 + 1.33261952385346*txt_uld/usse_load_ver - 25.0852804971041*txt_uld
	#power = 65.5004216379701 + 37.8730792054062*txt_uld + 2.59878654106738*fps - 5339.72818870851*usse_load_ver
	
	#latest one 18 hours run
	#power = 65.5004216379701 + 37.8730792054062*txt_uld + 2.59878654106738*fps - 5339.72818870851*usse_load_ver
	
	
	power[power<0]<-0

  #if(FALSE)
  if(TRUE)
  {
  	mean_async <- mean(asyncTable$power)
  	min_async  <- min(asyncTable$power)
  	totalAsync <- sum(asyncTable$power)
  	#cat("total ") 
  	#cat(totalAsync) 
  	#cat("\n")
  	#power <- power + mean_async
  	
  	l <- c()
  	
  	if(TRUE)
  	#if(FALSE)
    {
  		appLen <- length(app[,1])
  		
  		for(a in c(1:numCol))
  		{
  			for(i in c(1:appLen))
  			{		
  			
  			  #browser()
  			  colVpf <- which(colnames(asyncTable)=="vpf")
  			
  				b<-rbind(asyncTable[a,1:colVpf], app[i,c(which(colnames(app)=="ftime"), which(colnames(app)=="fps"),which(colnames(app)=="gtl3d_core"),which(colnames(app)=="gtlta_core"), which(colnames(app)=="ta_load"):which(colnames(app)=="vpf"))])
  			 
  				sim <- negDistMat(b,r=2)
  			 
  				if(sim[1,2] > -10000)
  				{
  					#cat("i=",i," sim=",sim[1,2],"\n")
  					#if not match
  					
  					async_pow <- asyncTable[a,2]
  					
  					if(length(l)==0)
  					{
  						l <- c(l,i)
  						power[i] <- power[i] + (async_pow/2)
              #cat(i)
              #cat(power[i])
              #("\n")
  						useAsync <- useAsync + (async_pow/2)
  					}
  					else
  					{
  						if(is.na(l[match(i,l)]))
  						{
  							l <- c(l,i)
  							power[i] <- power[i] + (async_pow/2)
  							#cat(i)
  							#cat("\n")
  							useAsync <- useAsync + (async_pow/2)
  						}
  					}
  				}
  			}
  			#cat("newLine \n")
  		}
  	}
  	
  	#cat(useAsync)
  	#cat("/")
  	#cat(totalAsync)
  	#cat("\n")
  	
  	ap <- totalAsync - useAsync
  	mean_unuse <- ap / numLine
  
  	power <- power + mean_unuse
  	#power = 148.120629824696 + 22170.0521183863*app$ftime + 60.7874838877534*app$txt_uld + 0.398938853694735*app$gta_core*app$vpf - 
  	#482.570166937158*app$ftime*app$vpf - 3.44839156171112*(app$gta_core^2) - 28718.7477425186*(app$ftime^2)
  }
  
	return(power)	
}


cstate <- 0 #low, 1 high
pstate <- 0
ttime <- 5
tstate <-0
power <- 0
tg_src <- function(app){
  
  
  #browser()
  appLen <- length(app[,1])
  
  for(i in c(1:appLen)){
    
    total <- app$tx[i]+app$rx[i]
    
    
    if(total > 230){
      cstate <- 1
    }
    else
    {
      cstate <- 0
    }
    
    if(ttime==5){
      if(cstate==0 && pstate==1) tstate<-1 else tstate <- 0
    }
    else{
      tstate=1
      
    }
    
    if(tstate==1){
      if(cstate==0) total<-230 
      power[i] <-  1.63 * total + 622
      ttime <- ttime - 1
      if(ttime == 0) ttime = 5
    }
    else
    {
      power[i] <-  1.63 * total + 622
      
    }
    pstate <- cstate
  }  
  
  return(power)
  
}


wifi_src <- function(app){ return(0) }

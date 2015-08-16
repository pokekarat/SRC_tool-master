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

if(!is.element("forecast", installed.packages()[,1]) )
{
  local({r <- getOption("repos")
  r["CRAN"] <- "http://cran.csie.ntu.edu.tw/"
  options(repos=r)})
  install.packages("forecast")
  
}else{ 
  print("forecast already installed.")
}


if(!is.element("zoo", installed.packages()[,1]) )
{
  local({r <- getOption("repos")
  r["CRAN"] <- "http://cran.csie.ntu.edu.tw/"
  options(repos=r)})
  install.packages("zoo")
  
}else{ 
  print("zoo already installed.")
}

require(apcluster)
require(Rcpp)
library("e1071")
library(forecast)
library(MASS)

asyncTable <- read.table("C:\\Users\\pok\\Dropbox\\Project1_SRC\\research\\s4\\gpu\\asyncTable.txt", header=TRUE, sep= " ")
totalAsync <- sum(asyncTable$power)
useAsync <- 0
numCol <- length(asyncTable[1,])
numLine <- length(asyncTable[,1])

cpu_lm <- function(app){
	
  #print(app)
  
	util0 <- app$util0
	
	#print(util0)
	freq0  <- app$freq0
	
	#ifelse(freq0 < 10000, freq0 <- freq0 * 1000, freq0)
	ifelse(freq0 > 10000, freq0 <- freq0 / 1000, freq0)
	
	it0s0  <- app$it0s0
	it0s1 <- app$it0s1
	it0s2 <- app$it0s2
	sumC0its <- (it0s0 + it0s1 + it0s2)+1
	
	print(sumC0its)
	
	iu0s0 <- app$iu0s0 + 1
	iu0s1 <- app$iu0s1 + 1
	iu0s2  <- app$iu0s2 + 1
	
	util1 <- app$util1
	util2  <- app$util2
	util3  <- app$util3

	#if(FALSE)
	#{	
		freq1  <- app$freq1
		freq2 <- app$freq2
		freq3  <- app$freq3
			
		it1s0  <- app$it1s0
		it1s1  <- app$it1s1
		it1s2 	 <- app$it1s2
		iu1s0  <- app$iu1s0
		iu1s1  <- app$iu1s1
		iu1s2  <- app$iu1s2
		
		it2s0  <- app$it2s0
		it2s1 <- app$it2s1
		it2s2 <- app$it2s2
		iu2s0  <- app$iu2s0
		iu2s1  <- app$iu2s1
		iu2s2  <- app$iu2s2
		
		it3s0 <- app$it3s0
		it3s1 <- app$it3s1
		it3s2  <- app$it3s2	
		iu3s0 <- app$iu3s0
		iu3s1 <- app$iu3s1
		iu3s2 <- app$iu3s2
	#}
	
	power <- -1
    b <- c()
	
  #print(freq0)
    
	ifelse(freq0 == 250,
		b <- c(179.98,3.09,0.19,0.31,735)
	,ifelse(freq0 == 300 || freq0 == 350,
		b <- c(-61.92,0.26,-0.17,0.34,740.2)
	,ifelse(freq0 == 400,
		b <- c(-0.69,12.34,0.08,0.34,734.212)
	,ifelse(freq0 == 500,
		b <- c(29.32,6.284,-0.06,0.52,746.63)
	,ifelse(freq0 == 600,
		b <- c(-13.13,4.92, -0.46,0.75,766.64)
	,ifelse(freq0 == 800,
		b <- c(-206.59,-6.09,0.01,-0.29,1162.54)
	,ifelse(freq0 == 900,	
		b <- c(-224.74,-15.96, 1.21,3.23,872.84)
	,ifelse(freq0 == 1000,
		b <- c(-270.44,40.69, 2.2,3.37,849.01)
	,ifelse(freq0 == 1100,
		b <- c(-132.57,-24.53, 0.47,3.32,884.69)
	,ifelse(freq0 == 1200,
		b <- c(-37.64,18.54, 2.7,3.36,835.711)
	,ifelse(freq0 == 1400,
		b <- c(0.189, 0.033,0.009,16.82,-40.40)
	,ifelse(freq0 == 1600,
		b <- c(0,0, 0,13.35,949)
	,0))))))))))))
	
	print(b)
	
	power <- (b[1]*((it0s0/sumC0its) * (it0s0/iu0s0))) + 
			 (b[2]*((it0s1/sumC0its) * (it0s1/iu0s1))) + 
			 (b[3]*((it0s2/sumC0its) * (it0s2/iu0s2))) +
			 (b[4] * util0) +
			 b[5]
	
	#print(power)
	
	avgUtil <- (util0 + util1 + util2 + util3)/4
	
	ifelse(freq0 < 1300, gap <- ((10 * avgUtil)/100), gap <- ((60 * avgUtil)/100))
	
	inc <- power * (gap/100)
	
	power2 <- power + inc
	
	#print(power2)
	
	return(power2)
}

screen_nlm <- function(app)
{
	bright <- app$bright
	r <- app$rc
	g <- app$gc
	b <- app$bc
	power <- ((0.003343*(bright^2))+(0.43 * r)+(0.23 * g)+(-0.033 * b))-46.93
	
	return(power)
}

bright_lm <- function(app){
	bright <- app$bright
	power <- (0.003343*(bright^2)) - 57.67
	return(power)
}

bright_src <- function(app)
{
	bright <- app$bright
	#power <- (0.003343*(bright^2)) - 57.67
	power <- 0.001*bright^2 - 1.76738794168994
	return(power)
}

pixelPower <- function(pix)
{
	power <- (0.42 * pix@rc) + (0.23 * pix@gc) + (-0.042 * pix@bc)
	return(power)
}

gpu_src <- function(app)
{	
	ftime <- app$ftime
	fps <- app$fps
	gtl2d <- app$gtl2d_core
	gtl3d <- app$gtl3d_core
	gtlcc <- app$gtac_core
	gtlta <- app$gtlta_core
	gtt2d <- app$gtt2d_core
	gtt3d <- app$gtt3d_core
	gttcc <- app$gttc_core
	gttta <- app$gttta_core
	spm <- app$spm
	isp <- app$isp
	tal <- app$ta_load
	usseccpp <- app$usse_cc_pp
	usseccpv <- app$usse_cc_pv
	usselp <- app$usse_load_p
	usselv <- app$usse_load_v
	vpf  <- app$vpf
	vps <- app$vps
		
	#power <- (188.84 * tal) + (-7.17 * fps) + (-1371.69 * usseccpp ) + 1828.59
	
	#power = 260.69 + 189.84*usselv + 102.84*gttta + 
	#		36.63*isp - 0.011*vps - 2.47*usseccpp - 
	#		9.10*usselp - 27.81*gtt3d
	
	#power = 569.610537379571 + 563.108102524359*sin(-12.8248164452516*fps)
	
	#power = 244.195995264296 + 1.73983167209346*gtlta - 22.5387582658645*isp
	
	#power = 243.955041991106 + 106.89405501114*gttta + 0.53323357651376*gtl3d - 
	#	    0.351680212889227*fps - 5.10102512963876*usselp - 26.4309363466159*gtt3d
	
	#power = 130.314868537057 + 3899.13061734714*gttta^4
	
	#power = 3573.90850278516*gttta^3 + 194.658018557383*cos(0.0294203588824125 + 1.79006653105577*gttta + 32.5504014922527*gttta^4) - 0.786099230354287*fps
	
	
	#power = 75.9793848889379/(44.2722324097179*usselv - gttta)
	#print(power)
	
	power = 232.267709646384 - fps
	
	#good
	#power = 603.763946234621 + 1702.98889570069*gtt3d^2 + 147.695282068048*gtt3d^4 - fps - 1370.89307188489*gtt3d - 857.657865456547*gtt3d^3

	#power = 149.587891727904 + 51.5712787841588/(usselp+1)
	
	power[power<0]<-0
	

	mean_async <- mean(asyncTable$power)
	min_async  <- min(asyncTable$power)
	totalAsync <- sum(asyncTable$power)
	
	l <- c()
		
	if(TRUE)
	{
		appLen <- length(app[,1])
		
		for(a in c(1:numCol))
		{
			for(i in c(1:appLen))
			{		
			
				colVpf <- which(colnames(asyncTable)=="vps")
			
				b<-rbind(asyncTable[1,1:colVpf],app[1,which(colnames(app)=="ftime"):which(colnames(app)=="vps")])
			 
				sim <- negDistMat(b,r=2)
			 
				if(sim[1,2] > -100) #-5000
				{
					#cat("i=",i," sim=",sim[1,2],"\n")
					#if not match
					
					async_pow <- asyncTable[a,2]
					
					if(length(l)==0)
					{
						l <- c(l,i)
						power[i] <- power[i] + (async_pow/2)
						useAsync <- useAsync + (async_pow/2)
					}
					else
					{
						if(is.na(l[match(i,l)]))
						{
							l <- c(l,i)
							power[i] <- power[i] + (async_pow/2)
							useAsync <- useAsync + (async_pow/2)
						}
					}
				}
			}
			#cat("newLine \n")
		}
	}

		
	ap <- totalAsync - useAsync
	mean_unuse <- ap / numLine
	power <- power + mean_unuse
	
	return(power)	
	
}

threeg_lm <- function(app){
  tx <- app$tx
  rx <- app$rx
  total = tx+rx
  power <- 0.484*(total) + 196.384 
}

cstate <- 0 #low, 1 high
pstate <- 0
ttime <- 5
tstate <-0
power <- 0
threeg_src <- function(app){
  
 
  #browser()
  appLen <- length(app[,1])
  
  for(i in c(1:appLen)){
    
    total <- app$tx[i]+app$rx[i]
    
    
    if(total > 300){
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
      if(cstate==0) total<-300 
      power[i] <- 0.484*(total) + 196.384
      ttime <- ttime - 1
      if(ttime == 0) ttime = 5
    }
    else
    {
      power[i] <- 0.484*(total) + 196.384
      
    }
    pstate <- cstate
  }  
  
  return(power)
  
}

wifi_lm <- function(app)
{

	tx <- app$tx
	rx <- app$rx
	
	power <- 4.098*tx + 1.003*rx - 33.819
	
	#11Mbps
	#power <- (-1.175  * tx) + ( 7.578   * rx) + 11.317
	
	#54Mbps
	#power <- (-0.4563  * tx) + ( 2.6633   * rx) + 3.7463
	
	#72 high error
	#power <- (-6.773  * tx) + ( 16.132   * rx) + -5.064
		
	return(power)
}

remove_outliers <- function(x, na.rm = TRUE, ...) 
{
    qnt <- quantile(x, probs=c(.25, .75), na.rm = na.rm, ...)
    H <- 1.5 * IQR(x, na.rm = na.rm)
    y <- x
    y[x < (qnt[1] - H)] <- NA
    y[x > (qnt[2] + H)] <- NA
    y
}
asyncTable <- read.table("Path to \\asyncTable.txt", header=TRUE, sep= " ")
totalAsync <- sum(asyncTable$power)
useAsync <- 0
numCol <- length(asyncTable[1,])
numLine <- length(asyncTable[,1])

async_power_estimate <- function(input)
{
	#A power model of this subsystem generated by Eureqa (Our research).
	
	mean_async <- mean(asyncTable$power)
	min_async  <- min(asyncTable$power)
	totalAsync <- sum(asyncTable$power)
	l <- c()
	
	inputLen <- length(input[,1])
	
	for(a in c(1:numCol))
	{
		for(i in c(1:inputLen))
		{		
		
			#Modify the "colNameStart" and "colNameEnd" which matches to your testing subsystem attributes
			colNameEnd <- which(colnames(asyncTable)=="colNameEnd")
			bind<-rbind(asyncTable[a,1:colNameEnd],input[i,which(colnames(input)=="colNameStart"):which(colnames(input)=="colNameEnd")])
		 
			sim <- negDistMat(bind,r=2)
		 
			#-100 is a threshold value, you can change it.
			if(sim[1,2] > -100)
			{
				
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
	}
	
	ap <- totalAsync - useAsync
	mean_unuse <- ap / numLine
	power <- power + mean_unuse
	
	return(power)	
}
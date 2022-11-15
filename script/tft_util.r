# n_epochs = 5
# n_batch_size = 128
# n_num_workers = 4
# time_colname="ds"
# target_colname="target"
# 
# unit = "hours"
# pred_len = 24*7
# lookback = pred_len*5
# validation = F
# learn_rate = 1e-3

# use_date_year = T
# use_date_month = T
# use_date_week = T
# use_date_day = T
# use_date_wday = T
# use_date_yday = T
# use_date_hour = T
# use_date_am = T
# use_date_pm = T

# window_size = 7
# use_date_lag = TRUE
# use_date_mean = TRUE
# use_date_sd = TRUE
# use_date_quantile  = TRUE

line_color_bule ="#00AFC5"
line_color_red ="#FF7042"

accelerator_default = luz::accelerator(
    device_placement = TRUE,
    cpu = FALSE,
    cuda_index = torch::cuda_current_device()
  )


tft_colname_conv <- function(df, time_colname, target_colname, key = "")
{
	if ( charmatch(time_colname,"date",nomatch = 0) == 0  )
	{
		df$date<- as.POSIXct(eval(parse(text=paste("df$", time_colname,sep=""))))
		eval(parse(text=paste(paste("df$", time_colname,sep=""),"<- NULL")))
	}else
	{
		df$date<- as.POSIXct(eval(parse(text=paste("df$", time_colname,sep=""))))
	}
	
	if (  charmatch(target_colname,"target",nomatch = 0) == 0)
	{
		df$target<-eval(parse(text=(paste("df$", target_colname,sep=""))))
		eval(parse(text=paste(paste("df$", target_colname,sep=""),"<-NULL")))
	}
	
	if ( charmatch(key,"",nomatch = 0) == 1 )
	{
		df$key <- as.factor(as.character(numeric(nrow(df))))
	}else
	{
		if ( charmatch(key,"key",nomatch = 0) == 0  )
		{
			df$key<-eval(parse(text=(paste("df$", key,sep=""))))
			eval(parse(text=paste(paste("df$", key,sep=""),"<-NULL")))
		}
		df$key <- as.factor(df$key)
	}
	
	print(table(is.na(df)) )
	
	df <- df %>%  group_by(key) %>%
	  		mutate(across(where(is.character), ~ as.factor(.x)))%>%
	  		mutate(across(where(is.integer), ~ as.numeric(.x)))
	
	tryCatch({
		df <- df %>%  group_by(key) %>%
	  		mutate(across(where(is.numeric), ~ replace_na(.x, mean(.x,na.rm = TRUE)))) 	
		},
		error = function(e) {
			print("replace_na error")
			print(e)
		},
		finally = {
			print("replace_na.")
		},
		silent = T
	 )	
	print(table(is.na(df)) )
	
	return (df)
}

tft_plot_input <- function(input_df, unit="week")
{
	p <- input_df %>% 
	  ggplot(aes(x = date, y = target, color=key))+
	  geom_line()+
	  scale_x_datetime(breaks = date_breaks(unit), labels = date_format("%Y-%m-%d %H")) +
	  theme(axis.text.x = element_text(angle = 45, hjust = 1))
	  
	plot(p)
	
	return(p)
}

tft_data_compact <- function(input_df, step_unit="week")
{
	#Aggregate data at weekly intervals to reduce computation time and make models more useful
	#Aggregate all variables using averages  
	
	input_df <- input_df %>% 
	  mutate(date = lubridate::floor_date(date, unit = step_unit)) %>% 
	  group_by(date, key) %>% 
	  summarise(across(everything(), .fns = ~mean(as.numeric(.x), na.rm = TRUE)), .groups = "drop")

#	input_df <- input_df %>% 
#	  mutate(date = lubridate::floor_date(date, unit = step_unit)) %>% 
#	  group_by(date, key) %>% 
#	  summarise(across(where(is.numeric), ~ mean(.x), .groups = "drop"))

	print(table(is.na(input_df)) )
	
	input_df <- input_df %>%  group_by(key) %>%
	  mutate(across(where(is.numeric), ~ replace_na(.x, mean(.x,na.rm = TRUE)))) 	
	print(table(is.na(input_df)) )
	
	  
	 return (input_df)
}


library(slider)
quantile25 <- function(x)
{
	return (as.numeric(quantile(x)[2]))
}
quantile75 <- function(x)
{
	return (as.numeric(quantile(x)[4]))
}
tft_data_split <- function(input_df, unit, lookback, pred_len, future_test_len, validation=F)
{
	#print(pred_len)
	train <- NULL
	valid <- NULL
	test <- NULL

	tidx <- input_df %>%  group_by(key) %>%  mutate(tidx = row_number())
	tidx <- tidx$tidx
	

	data_tbl <- input_df
	if ( unit == "hour" )
	{
		if ( !is.na(lubridate::ymd_hms(min(input_df$date))[1]))
		{
			data_tbl <- input_df %>%
			  mutate(
				self_adding_date_time_since_bg = 0.0001*as.numeric(difftime(
				      time1 = date, 
				      time2 = lubridate::ymd_hms(min(date)), 
				      units = "auto"
				    ))
			  )
		}else
		{
			data_tbl <- input_df %>%
			  mutate(
				self_adding_date_time_since_bg = 0.0001*as.numeric(difftime(
				      time1 = date, 
				      time2 = lubridate::ymd(min(date)), 
				      units = "auto"
				    ))
			    )
		}
	    data_tbl$self_adding_date_month = as.factor(lubridate::month(input_df$date))
	    data_tbl$self_adding_date_wday = as.factor(lubridate::wday(input_df$date))
	    data_tbl$self_adding_date_day = as.factor(lubridate::day(input_df$date))
	    data_tbl$self_adding_date_hour = as.factor(lubridate::hour(input_df$date))
	    data_tbl$self_adding_date_am = as.factor(lubridate::am(input_df$date))
	    data_tbl$self_adding_date_pm = as.factor(lubridate::pm(input_df$date))
	    data_tbl$self_adding_date_yday = as.factor(lubridate::yday(input_df$date))
	    data_tbl$self_adding_date_quarter = as.factor(lubridate::quarter(input_df$date))
	    data_tbl$self_adding_date_sin_Y = sin(2*pi*tidx/8760)
	    data_tbl$self_adding_date_cos_Y = cos(2*pi*tidx/8760)
	    data_tbl$self_adding_date_sin_M = sin(2*pi*tidx/730.001)
	    data_tbl$self_adding_date_cos_M = cos(2*pi*tidx/730.001)
	    data_tbl$self_adding_date_sin_W = sin(2*pi*tidx/168.0)
	    data_tbl$self_adding_date_cos_W = cos(2*pi*tidx/168.0)
	    data_tbl$self_adding_date_sin_D = sin(2*pi*tidx/24.0)
	    data_tbl$self_adding_date_cos_D = cos(2*pi*tidx/24.0)
	}
	
	# as.Date()("1960-04-01"->"1960-03-31") -> lubridate::as_date()("1960-04-01"->"1960-04-01")
	if ( unit == "day" )
	{
		input_df$date <- lubridate::as_date(input_df$date)
		data_tbl <- input_df %>%
		  mutate(
			self_adding_date_time_since_bg = 0.0001*as.numeric(difftime(
			      time1 = date, 
			      time2 = lubridate::ymd(min(date)), 
			      units = "auto"
			    ))
		   )
	    data_tbl$self_adding_date_month = as.factor(lubridate::month(input_df$date))
	    data_tbl$self_adding_date_week = as.factor(lubridate::week(input_df$date))
	    data_tbl$self_adding_date_wday = as.factor(lubridate::wday(input_df$date))
	    data_tbl$self_adding_date_day = as.factor(lubridate::day(input_df$date))
	    data_tbl$self_adding_date_yday = as.factor(lubridate::yday(input_df$date))
	    data_tbl$self_adding_date_quarter = as.factor(lubridate::quarter(input_df$date))
	    data_tbl$self_adding_date_sin_Y = sin(2*pi*tidx/365)
	    data_tbl$self_adding_date_cos_Y = cos(2*pi*tidx/365)
	    data_tbl$self_adding_date_sin_M = sin(2*pi*tidx/30.4167)
	    data_tbl$self_adding_date_cos_M = cos(2*pi*tidx/30.4167)
	    data_tbl$self_adding_date_sin_W = sin(2*tidx/7)
	    data_tbl$self_adding_date_cos_W = cos(2*tidx/7)
	}
	if ( unit == "week" )
	{
		input_df$date <- lubridate::as_date(input_df$date)
		data_tbl <- input_df %>%
		  mutate(
			self_adding_date_time_since_bg = 0.0001*as.numeric(difftime(
			      time1 = date, 
			      time2 = lubridate::ymd(min(date)), 
			      units = "auto"
			    ))
		  )

	    data_tbl$self_adding_date_year = as.factor(lubridate::year(input_df$date))
	    data_tbl$self_adding_date_month = as.factor(lubridate::month(input_df$date))
	    data_tbl$self_adding_date_week = as.factor(lubridate::week(input_df$date))
	    data_tbl$self_adding_date_yday = as.factor(lubridate::yday(input_df$date))
	    data_tbl$self_adding_date_quarter = as.factor(lubridate::quarter(input_df$date))
	    data_tbl$self_adding_date_sin_Y = sin(2*pi*tidx/52.1429)
	    data_tbl$self_adding_date_cos_Y = cos(2*pi*tidx/52.1429)
	    data_tbl$self_adding_date_sin_M = sin(2*pi*tidx/4.34524)
	    data_tbl$self_adding_date_cos_M = cos(2*pi*tidx/4.34524)
	}
	if ( unit == "month" )
	{
		input_df$date <- lubridate::as_date(input_df$date)
		data_tbl <- input_df %>%
		  mutate(
			self_adding_date_time_since_bg = 0.0001*as.numeric(difftime(
			      time1 = date, 
			      time2 = lubridate::ymd(min(date)), 
			      units = "auto"
			    ))
		  )
	    data_tbl$self_adding_date_year = as.factor(lubridate::year(input_df$date))
	    data_tbl$self_adding_date_month = as.factor(lubridate::month(input_df$date))
	    data_tbl$self_adding_date_yday = as.factor(lubridate::yday(input_df$date))
	    data_tbl$self_adding_date_quarter = as.factor(lubridate::quarter(input_df$date))
	    data_tbl$self_adding_date_sin_Y = sin(2*pi*tidx/12)
	    data_tbl$self_adding_date_cos_Y = cos(2*pi*tidx/12)
	}
	if ( unit == "year" )
	{
		input_df$date <- lubridate::as_date(input_df$date)
		data_tbl <- input_df %>%
		  mutate(
			self_adding_date_time_since_bg = 0.0001*as.numeric(difftime(
			      time1 = date, 
			      time2 = lubridate::ymd(min(date)), 
			      units = "auto"
			    ))
		  )
	    data_tbl$self_adding_date_year = as.factor(lubridate::year(input_df$date))
	    data_tbl$self_adding_date_quarter = as.factor(lubridate::quarter(input_df$date))
	}
   if (!use_date_year && ("self_adding_date_year" %in% names(data_tbl))) data_tbl$self_adding_date_year <- NULL
   if (!use_date_month && ("self_adding_date_month" %in% names(data_tbl))) data_tbl$self_adding_date_month <- NULL
   if (!use_date_week && ("self_adding_date_week" %in% names(data_tbl))) data_tbl$self_adding_date_week <- NULL
   if (!use_date_wday && ("self_adding_date_wday" %in% names(data_tbl))) data_tbl$self_adding_date_wday <- NULL
   if (!use_date_day && ("self_adding_date_day" %in% names(data_tbl))) data_tbl$self_adding_date_day <- NULL
   if (!use_date_yday && ("self_adding_date_yday" %in% names(data_tbl))) data_tbl$self_adding_date_yday <- NULL
   if (!use_date_hour && ("self_adding_date_hour" %in% names(data_tbl))) data_tbl$self_adding_date_hour <- NULL
   if (!use_date_am && ("self_adding_date_am" %in% names(data_tbl))) data_tbl$self_adding_date_am <- NULL
   if (!use_date_pm && ("self_adding_date_pm" %in% names(data_tbl))) data_tbl$self_adding_date_pm <- NULL
   if (!use_date_quarter && ("self_adding_date_quarter" %in% names(data_tbl))) data_tbl$self_adding_date_quarter <- NULL
   
   if (!use_date_sincosY &&  ("self_adding_date_sin_Y" %in% names(data_tbl))) data_tbl$self_adding_date_sin_Y <- NULL
   if (!use_date_sincosY &&  ("self_adding_date_cos_Y" %in% names(data_tbl))) data_tbl$self_adding_date_cos_Y <- NULL
   if (!use_date_sincosM &&  ("self_adding_date_sin_M" %in% names(data_tbl))) data_tbl$self_adding_date_sin_M <- NULL
   if (!use_date_sincosM &&  ("self_adding_date_cos_M" %in% names(data_tbl))) data_tbl$self_adding_date_cos_M <- NULL
   if (!use_date_sincosW &&  ("self_adding_date_sin_W" %in% names(data_tbl))) data_tbl$self_adding_date_sin_W <- NULL
   if (!use_date_sincosW &&  ("self_adding_date_cos_W" %in% names(data_tbl))) data_tbl$self_adding_date_cos_W <- NULL
   if (!use_date_sincosD &&  ("self_adding_date_sin_D" %in% names(data_tbl))) data_tbl$self_adding_date_sin_D <- NULL
   if (!use_date_sincosD &&  ("self_adding_date_cos_D" %in% names(data_tbl))) data_tbl$self_adding_date_cos_D <- NULL
	
	#print(window_size)
	#print(pred_len)
	if ( window_size > pred_len )
	{
	    data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_date_lag = lag(target, n=window_size ))
	    if ( use_date_mean )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>% mutate(self_adding_date_mean =  slide_vec(.x = self_adding_date_lag, .f = mean, .before = window_size))
		}
	    if ( use_date_sd )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_date_sd =  slide_vec(.x = self_adding_date_lag, .f = sd, .before = window_size))
		}
	    if ( use_date_min )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_date_min =  slide_vec(.x = self_adding_date_lag, .f = min, .before = window_size))
		}
	    if ( use_date_max )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_date_max =  slide_vec(.x = self_adding_date_lag, .f = max, .before = window_size))
		}
		data_tbl <- data_tbl %>%  group_by(key) %>%
	  		mutate(across(where(is.numeric), ~ replace_na(.x, mean(.x,na.rm = TRUE)))) 	

	    if ( use_date_quantile )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_date_quantile25 =  slide_vec(.x = self_adding_date_lag, .f = quantile25, .before = window_size))
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_date_quantile75 =  slide_vec(.x = self_adding_date_lag, .f = quantile75, .before = window_size))
		}
	    if ( !use_date_lag )
	    {
	    	data_tbl$self_adding_statistics_lag <- NULL
		}
	}else
	{
	    data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_statistics_lag = lag(target, n=window_size ))
	    if ( use_date_mean )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>% mutate(self_adding_statistics_mean =  slide_vec(.x = self_adding_statistics_lag, .f = mean, .before = window_size))
		}
	    if ( use_date_sd )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_statistics_sd =  slide_vec(.x = self_adding_statistics_lag, .f = sd, .before = window_size))
		}
	    if ( use_date_min )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_statistics_min =  slide_vec(.x = self_adding_statistics_lag, .f = min, .before = window_size))
		}
	    if ( use_date_max )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_statistics_max =  slide_vec(.x = self_adding_statistics_lag, .f = max, .before = window_size))
		}
		data_tbl <- data_tbl %>%  group_by(key) %>%
	  		mutate(across(where(is.numeric), ~ replace_na(.x, mean(.x,na.rm = TRUE)))) 	

	    if ( use_date_quantile )
	    {
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_statistics_quantile25 =  slide_vec(.x = self_adding_statistics_lag, .f = quantile25, .before = window_size))
			data_tbl<-data_tbl %>%  group_by(key) %>%  mutate(self_adding_statistics_quantile75 =  slide_vec(.x = self_adding_statistics_lag, .f = quantile75, .before = window_size))
		}
	    if ( !use_date_lag )
	    {
	    	data_tbl$self_adding_statistics_lag <- NULL
		}
	}
	
	head(data_tbl)
	str(data_tbl)
	out.file <- file("tmp_tft_data_split.r", open = "w")

	last_date <- max(data_tbl$date)
	writeLines("last_date <- max(data_tbl$date)", out.file)

	lubridate_namespace = "lubridate::"
	if ( unit == "month" ) lubridate_namespace = ""
	
	#As your tft_dataset_spec() includes known covariates, 
	#you must provide them for the model to train. 
	#So the minimum number of observations must be lookback + horizon.
	if (validation)
	{
		s = sprintf("train <- data_tbl %%>%% filter(date <= (last_date - %s%ss((lookback+future_test_len+pred_len))))", lubridate_namespace, unit)
		writeLines(s, out.file)
		
		s = sprintf("valid <- data_tbl %%>%% filter(date > (last_date - %s%ss((lookback+future_test_len+pred_len))),date <= (last_date - %s%ss((future_test_len+pred_len))))", lubridate_namespace, unit, lubridate_namespace, unit)
		writeLines(s, out.file)

		s = sprintf("test <- data_tbl %%>%% filter(date > (last_date - %s%ss((future_test_len+pred_len))))", lubridate_namespace, unit)
		writeLines(s, out.file)
		
	}else
	{
		s = sprintf("train <- data_tbl %%>%% filter(date <= (last_date - %s%ss((future_test_len+pred_len))))", lubridate_namespace, unit)
		writeLines(s, out.file)
		
		s = sprintf("test <- data_tbl %%>%% filter(date > (last_date - %s%ss((future_test_len+pred_len))))", lubridate_namespace, unit)
		writeLines(s, out.file)
	
		valid <- NULL
		writeLines("valid <- NULL", out.file)
	}
	writeLines("#keynum <- length(unique(input_df$key))", out.file)
	writeLines("#pred_len <- nrow(test)/keynum", out.file)
	writeLines("#print(train)", out.file)
	writeLines("#print(valid)", out.file)
	writeLines("#print(test)", out.file)
	close(out.file)
	
	return (data_tbl)
}

tft_make_recipe <- function(train, validation=F, ...)
{
	#en
	#Interpolate missing values by mean (step_impute_mean)
	#Remove columns with identical values (step_zv)
	#Remove columns with almost identical values (step_nzv)
	#Rows with missing values are deleted (step_naomit)
	
	#jp
	#欠損値を平均で補間(step_impute_mean)
	#同一値の列を削除(step_zv)
	#ほとんど同一値の列を削除(step_nzv)
	#欠損値がある行を削除(step_naomit)
	
	rec <- recipe(target ~ ., data = train) %>% 
	  step_impute_mean(all_numeric_predictors()) %>%
	  step_normalize(all_numeric_predictors())

	return (rec)
}

tft_make_spec <- function(train, rec, covariate_known=NULL, covariate_static=NULL)
{
	  
	#inde, key,known is required
	#spec <- tft_dataset_spec(rec, train)  %>% 
	#   spec_covariate_index(date)  %>% 
	#   spec_covariate_key(key) %>%
	#   spec_covariate_known(x5, x6, x7, starts_with("self_adding_date_"))

	out.file <- file("tmp_tft_make_spec.r", open = "w")
	writeLines("spec <- NULL", out.file)
	if ( validation )
	{
		writeLines("spec <- tft_dataset_spec(rec, bind_rows(train,valid)) %>% ", out.file)
	}else
	{
		writeLines("spec <- tft_dataset_spec(rec, train) %>% ", out.file)
	}
	writeLines("    spec_covariate_index(date) %>% ", out.file)
	writeLines("    spec_covariate_key(key) %>% ", out.file)
	writeLines("    spec_covariate_known(starts_with(\"self_adding_date_\")", out.file)
	if ( length(covariate_known) > 0 )
	{
		for ( i in 1:length(covariate_known) )
		{
			writeLines(paste("                    ,", covariate_known[i]), out.file)
		}
	}
	if ( length(covariate_static) > 0 )
	{
		writeLines("                            ) %>% ", out.file)
	}else
	{
		writeLines("                            )", out.file)
	}
	if ( length(covariate_static) > 0 )
	{
		writeLines("    spec_covariate_static(", out.file)
		writeLines(paste("                    ", covariate_static[1]), out.file)
		if ( length(covariate_static) > 0 )
		{
			for ( i in 2:length(covariate_static) )
			{
				writeLines(paste("                    ,", covariate_static[i]), out.file)
			}
		}
		writeLines("                            )", out.file)
	}
	
	writeLines("", out.file)
	writeLines("#lookback :Number of time steps used as historical data for forecasting", out.file)
	writeLines("#horizon :Number of time steps predicted by the model", out.file)
	writeLines("spec <- spec %>% ", out.file)
	writeLines("  spec_time_splits(lookback = lookback, horizon = pred_len)", out.file)
	writeLines("", out.file)
	writeLines("spec <- prep(spec)", out.file)
	close(out.file)
}


lookup_lr <- function(spec, train, accelerator = accelerator_default, steps = 100, return_plot = F)
{
	model <- temporal_fusion_transformer(
	  spec, 
	  hidden_state_size = 8,
	  learn_rate = learn_rate, 
	  dropout = 0.15, 
	  num_attention_heads = 1, 
	  num_lstm_layers = 1
	)
	#Find the optimal learning rate for the model
	result<- NULL
	result <- luz::lr_finder(
	  model, 
	  transform(spec, train), 
	  start_lr = 1e-4, 
	  end_lr = 0.9,
	  dataloader_options = list(
	    batch_size = n_batch_size
	  ),
	  steps = steps,
      accelerator = accelerator,
	  verbose = T
	)
	p <- plot(result) + ggplot2::coord_cartesian(ylim = c(0.0, 1.5))
	
	print(result)
	idx <- -1
	idx <- which.min(result$loss)
	
	if ( return_plot ) return (list (result$lr[idx], p))
	return (result$lr[idx])
}

may_callback <- luz::luz_callback(
	 name = "may_callback",
	 on_train_batch_end = function() {
	   #cat("Iteration ", ctx$iter, "\n")
	 },
	 on_epoch_end = function() {
	   #cat("Done!\n")
	 }
)

tft_train_ <- function(spec, accelerator = accelerator_default, base_name = "", validation=FALSE, valid = NULL)
{
	options(timeout= Inf)
	setTimeLimit(Inf)
	setSessionTimeLimit(Inf)
	
	logfile = paste(base_name, "_train.log")
	if (file.exists(logfile))
	{
		file.remove(logfile)
	}	
	
	#Fitting a model
	model <- temporal_fusion_transformer(
	  spec, 
	  hidden_state_size = 8,
	  learn_rate =  0.001, 
	  dropout = 0.15, 
	  num_attention_heads = 1, 
	  num_lstm_layers = 1
	)

	fitted<- NULL
	if (validation )
	{
		fitted <- model %>% 
		  fit(
		    transform(spec),
		    epochs = n_epochs,
		    verbose = T,

		    valid_data = transform(spec, new_data = valid),
		    callbacks = list(
		      luz::luz_callback_keep_best_model(monitor = "valid_loss"),
		      luz::luz_callback_early_stopping(
		        monitor = "valid_loss", 
		        patience = 5, 
		        min_delta = 0.001
		      ),
		      luz::luz_callback_csv_logger(logfile),
		      may_callback()
		    ),
		    accelerator = accelerator,
		    dataloader_options = list( batch_size = n_batch_size, num_workers = n_num_workers)
		  )
	}else
	{
		fitted <- model %>% 
		  fit(
		    transform(spec),
		    epochs = n_epochs,
		    verbose = T,
			callbacks = list(
			    luz::luz_callback_keep_best_model(monitor = "train_loss"),
			    luz::luz_callback_early_stopping(monitor = "train_loss", patience = 5, 
			                                     min_delta = 0.001),
		        luz::luz_callback_csv_logger(logfile),
       		    may_callback()
 		    ),
		    accelerator = accelerator,
	        dataloader_options = list( batch_size = n_batch_size, num_workers = n_num_workers)
	      )
	}
	return (fitted)
}

tft_train <- function(spec, accelerator=accelerator_default, validation=FALSE, valid = NULL, base_name = "")
{
	tryCatch({
			fitted <- tft_train_(spec, accelerator=accelerator, base_name=base_name)
		},
		error = function(e) {
			print("tft_train error")
			print(e)
			err.log <- file(paste("tft_train_errorLog_", base_name, ".txt", sep=""), open = "w")
			writeLines(paste(Sys.time(), e, sep=" "), err.log)
			close(err.log)

			print(fitted)
			#if (file.exists(tmpfile))
			#{
			#	fitted <-luz::luz_load(tmpfile)
			#}
		},
		finally = {
			print("tft finish.")
		},
		silent = T
	 )
	 return ( fitted )
}






#x <- test %>% mutate(target = NA_real_)

tft_predict <- function(fitted, test, validation=F, base_name="")
{
	#Evaluate models in the test set
	
	#Loading a saved model results in an error.
	#cpp_torch_tensor_dtype(self$ptr) error: external pointer is not val
# 	if ( validation )
# 	{
# 		fitted %>% 
# 		  luz::evaluate(
# 		    transform(spec, new_data = test, past_data = rbind(train, valid))
# 		  )
# 	}else
# 	{
# 		fitted %>% 
# 		  luz::evaluate(
# 		    transform(spec, new_data = test, past_data = train)
# 		  )
# 	}
	#forecasts <- generics::forecast(fitted, past_data = bind_rows(train, test))


	tmp_pred <- NULL
	n = length(unique(input_df$key))
	
	tryCatch({
		if ( nrow(test)/n > pred_len )
		{
			s = 1
			e = pred_len*n
			#print(s)
			#print(e)
			if ( validation )
			{
				past <- bind_rows(train, valid)
			}else
			{
				past <- train
			}
			tmp <- test[s:e,]
			#print(head(tmp,12))
			#print(nrow(tmp))
			while(e <= nrow(test))
			{
				#print(e)
				pred <- NULL
				tryCatch({
							pred <- predict(object = fitted, new_data = tmp, past_data = past,mode = "full")
				}
	  			, error = function(e) { 
					print("tft_predict error")
					print(e)
					err.log <- file(paste("tft_predict_errorLog_", base_name, ".txt", sep=""), open = "w")
					writeLines(paste(Sys.time(), e, sep=" "), err.log)
					close(err.log)
	  			 }
				)
				if ( is.null(pred) )
				{
					tmp_pred = NULL
					break
				} 
				tmp$target <- pred$.pred
				tmp_pred <- bind_rows(tmp_pred, pred[(nrow(pred)- n*length(s:e)+1):nrow(pred),])
				
				if ( e == nrow(test)) break
				past <- bind_rows(past, tmp)
				s = e + 1
				e = (s-1) + pred_len*n
				
				if ( e > nrow(test))
				{
					e = nrow(test)
				}
				tmp <- test[s:e,]
				nrow(tmp)
				if ( length(s:e)/n < pred_len )
				{
					d = pred_len - length(s:e)/n
					t = past[(nrow(past)-d*n+1):nrow(past),]
					tmp = bind_rows(t, test[s:e,])
					past = past[1:(nrow(past)-d*n),]
				}
				
				#print(s)
				#print(e)
			}
		}else
		{
			if ( validation )
			{
				#pred <- predict(object = fitted, new_data = test, past_data = valid)
				pred <- predict(object = fitted, new_data = test, past_data = bind_rows(train, valid), mode = "full")
			}else
			{
				pred <- predict(object = fitted, new_data = test, past_data = train)
			}
			tmp_pred = pred
		}
	},
	error = function(e)
	{
			print("tft_predict error")
			print(e)
			err.log <- file(paste("tft_predict_errorLog_", base_name, ".txt", sep=""), open = "w")
			writeLines(paste(Sys.time(), e, sep=" "), err.log)
			close(err.log)
			
			tmp_pred = NULL
	},
	finally = {
		if ( is.null(tmp_pred) ){
			pred = NULL
		}else {
			pred <- bind_cols(test[1:nrow(tmp_pred),], tmp_pred)
		}
	},
    silent = TRUE
    )

	return (pred)
}

tft_predict_plot <- function(pred, timestep="week", use_real_data = FALSE)
{
	n = 6*lookback
	if ( nrow(train) <= n ) n = 5*lookback
	if ( nrow(train) <= n ) n = 4*lookback
	if ( nrow(train) <= n ) n = 3*lookback
	if ( nrow(train) <= n ) n = lookback/2
	
	t <- train

	if ( !is.null(valid) )
	{
		t <- bind_rows(train, valid)
	}

	pred2 <- pred %>% mutate(target = NA_real_)	
	if (use_real_data)
	{
		pred2 <- pred
	}


	predict_df <- t[(nrow(t)- n):nrow(t),] %>% 
	  full_join(pred2)

	plt <- NULL
	if ( unit == "year" || unit == "week" || unit == "month" || unit == "day")
	{
		plt <- predict_df %>% 
		  ggplot(aes(x = date, y = target)) +
		  geom_line(color = line_color_bule) +
		  geom_line(aes(y = .pred), color = line_color_red) +
		  geom_ribbon(aes(ymin = .pred_lower, ymax = .pred_upper), alpha = 0.3)+
		  #scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d")) +
		  theme(axis.text.x = element_text(angle = 45, hjust = 1)) +
		  facet_wrap(~key)
	}else
	{
		plt <- predict_df %>% 
		  ggplot(aes(x = date, y = target)) +
		  geom_line(color = line_color_bule) +
		  geom_line(aes(y = .pred), color = line_color_red) +
		  geom_ribbon(aes(ymin = .pred_lower, ymax = .pred_upper), alpha = 0.3)+
		  scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d %H")) +
		  theme(axis.text.x = element_text(angle = 45, hjust = 1)) +
		  facet_wrap(~key)
	}
	
	return(plt)
}
  
tft_predict_plot_ymd <- function(pred, cutoff_ymd="2019-01-01")
{
	t <- train
	if ( !is.null(valid) )
	{
		t <- bind_rows(train, valid)
	}
	pred2 <- pred %>% mutate(target = NA_real_)	

	plt <- t %>% 
	  filter(date > lubridate::ymd(cutoff_ymd)) %>% 
	  full_join(pred2) %>% 
	  ggplot(aes(x = date, y = target)) +
	  geom_line(color = line_color_bule) +
	  geom_line(aes(y = .pred), color = line_color_red) +
	  geom_ribbon(aes(ymin = .pred_lower, ymax = .pred_upper), alpha = 0.3) +
	  facet_wrap(~key)

	return(plt)
}
  
tft_predict_check <- function(pred, timestep="day")
{
	plt <- NULL
	if ( unit == "week" || unit == "month" || unit == "day")
	{
		plt <- pred %>% 
		  ggplot(aes(x = date, y = target)) +
		  geom_line(color = line_color_bule) +
		  geom_line(aes(y = .pred), color = line_color_red) +
		  geom_ribbon(aes(ymin = .pred_lower, ymax = .pred_upper), alpha = 0.2)+
		  #scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d")) +
		  theme(axis.text.x = element_text(angle = 45, hjust = 1)) +
		  facet_wrap(~key)
	}else
	{
		plt <- pred %>% 
		  ggplot(aes(x = date, y = target)) +
		  geom_line(color = line_color_bule) +
		  geom_line(aes(y = .pred), color = line_color_red) +
		  geom_ribbon(aes(ymin = .pred_lower, ymax = .pred_upper), alpha = 0.2)+
		  scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d %H")) +
		  theme(axis.text.x = element_text(angle = 45, hjust = 1)) +
		  facet_wrap(~key)
	}
	return (plt)
	
}

tft_pred_save <- function(pred, filename="pred_save.csv")
{
	write.csv(pred, filename)
}

#install.packages("gplots")
#install.packages("tidymodels")
#install.packages("rlang")
library(ggplot2)
library(gplots)

# The permutation feature importance algorithm based on Fisher, Rudin, and Dominici (2018) 
permutationFeatureImportance<- function(fitted, test, validation=F, base_name="")
{
	n = length(unique(input_df$key))

	test_tmp <- test[1:pred_len*n,]
	date_idx <- which( colnames(test_tmp)=='date' )
	target_idx <- which( colnames(test_tmp)=='target' )
	key_idx <- which( colnames(test_tmp)=='key' )
	n = ncol(test_tmp)
	
	test_tmp0 <- test_tmp

	pred0 <- tft_predict(fitted, test_tmp, validation=validation, base_name=base_name)
	
	measure <- tft_predict_measure(pred0)[[1]]
	mse0 <- measure$MSE

	FI = NULL
	FI_s = NULL
	sampling_n = 10
	
	for ( k in 1:sampling_n )
	{
		name = c(1:(n-3))*0
		mse = c(1:(n-3))*0
		preds = test_tmp$date

		id = 1
		for ( i in 1:n )
		{
			test_tmp <- test_tmp0
			if ( i == target_idx ) next
			if ( i == date_idx ) next
			if ( i == key_idx ) next
			test_tmp[,i] <- test_tmp[order(rnorm(nrow(test_tmp[,i]))),i]
			
			pred <- tft_predict(fitted, test_tmp, validation=validation, base_name=base_name)
			preds <- cbind(preds, abs(pred$.pred - (pred$target)))

			mse[id] <- tft_predict_measure(pred)[[1]]$MSE
			name[id] <- colnames(test_tmp)[i]
			id = id + 1
		}
		
		s = 0
		if ( k > 1 )
		{
			s = FI$feature_importance
		}
			
		
		FI <- data.frame(feature_importance=abs(mse-mse0)+s, name=c(name))
		
		colnames(preds)[1]<- c('date')
		colnames(preds)[2:ncol(preds)]<- c(name)
		FI_s<-as.data.frame(preds)
		FI_s$date <- test_tmp$date
		rownames(FI_s) <- FI_s$date
		FI_s$date<- NULL
		
		if ( k > 1 )
		{
			FI_s = FI_s2 + FI_s
		}
		FI_s2 = FI_s
	}
	FI_s2 = FI_s2 / sampling_n
	FI$'feature_importance' = FI$'feature_importance'/ sampling_n
	
	FI <- FI[order(FI$'feature_importance',decreasing=T),]

	g1 <- ggplot(FI, aes(x = reorder(name, feature_importance), y = feature_importance, fill = name))
	g1 <- g1 + geom_bar(stat = "identity")
	g1 <- g1 + coord_flip()
	g1 <- g1 + xlab('covariate')
	plot(g1)
	ggsave(file = paste(base_name,"_feature_importance.png", sep=""), plot = g1, dpi = 100, width = 6.4, height = 4.8*length(name)/10)




	FI_s2 <- FI_s
	for(i in 1:nrow(FI_s2))
  	{
  		x <- as.matrix(FI_s[i,])
		mn = min(x)
		mx = max(x)
  		y <- (x - mn)/(mx-mn)
  		FI_s2[i,] <- y
 	}
 	FI_s2$date <- test_tmp$date
 	
 	x<-horizontally_to_vertically(FI_s2, ids_cols=c('date'), key=name)
 	x$importance <- x$target
 	x$target <- NULL
	g2 <- ggplot(data = x, aes(x = date, y = key , fill = importance)) + 
	geom_tile()+
	scale_fill_gradient2(low = "springgreen4", mid = "yellow", high = "red", midpoint = 0.5)
	ggsave(file = paste(base_name,"_feature_importance_time.png", sep=""), plot = g2, dpi = 100, width = 6.4, height = 4.8*length(name)/10)
 	
 	if ( FALSE )
 	{
		
	 	FI_s2$date <- NULL
	 	FI_s2 = t(FI_s2)
		#par(mar = c(8.5, 1.0, 1.1, 5)) #  余白の広さを行数で指定．下，左，上，右の順．
		heatmap(as.matrix(FI_s2),Colv = NA, Rowv=NA, scale='col',col=bluered(256))

		heatmap(as.matrix(FI_s2),Colv = NA, Rowv=NA, scale='col',col=bluered(256))
		heatmap(as.matrix(FI_s2),Colv = NA, Rowv=NA, scale='col',col=greenred(256))
		heatmap(as.matrix(FI_s2),Colv = NA, Rowv=NA, scale='col',col=c(rgb(seq(0.9,0,-0.001), 0, 0)))
		heatmap(as.matrix(FI_s2),Colv = NA, Rowv=NA, scale='col',col=c(rgb(0,seq(0.9,0,-0.001), 0)))
		heatmap(as.matrix(FI_s2),Colv = NA, Rowv=NA, scale='col',col=c(rgb(seq(0.9,0.2,-0.001),0, seq(0.0,0.3,0.001))))
		heatmap(as.matrix(FI_s2),Colv = NA, Rowv=NA, scale='col',col=c(rgb(seq(0.9,0.2,-0.001),0, seq(0.0,0.2,0.001))))
	}
	return( list(n, g1, g2))
}


tft_predict_measure <- function(pred)
{
	summary1 <- pred %>%  
	summarise(count = n(),
             MSE=sum(target-.pred)^2/count,
             RMSE=sqrt(sum(target-.pred)^2/count),
             MAE =sum(abs(target-.pred))/count,
             MER =median(abs(target-.pred)/target),
             MAPE=100*sum(abs(target-.pred)/target)/count,
             MSEL=sum((log(1+target)-log(1+.pred))^2)/count,
             RMSEL=sqrt(sum((log(1+target)-log(1+.pred))^2)/count))

	summary2 <- pred %>%  group_by(key) %>%
	summarise(count = n(),
             MSE=sum(target-.pred)^2/count,
             RMSE=sqrt(sum(target-.pred)^2/count),
             MAE =sum(abs(target-.pred))/count,
             MER =median(abs(target-.pred)/target),
             MAPE=100*sum(abs(target-.pred)/target)/count,
             MSEL=sum((log(1+target)-log(1+.pred))^2)/count,
             RMSEL=sqrt(sum((log(1+target)-log(1+.pred))^2)/count))
             
    print(summary1)
    print(summary2)
    return(list(summary1,summary2))
}

#ids_cols:en:Column name string to be fixed (jp:固定する列名文字列) example: c("date_time", "deg_C", "relative_humidity")
#key_cols:en:Column name strings lined up for each column you want to arrange vertically (jp:縦に並べたい列毎に並んでいる列名文字列) example:c("target_carbon_monoxide", "target_benzene","target_nitrogen_oxides")
horizontally_to_vertically <- function(df, ids_cols, key_cols)
{
	df2 <- reshape2::melt(df, id.vars=ids_cols, measure.vars=key_cols, 
				variable.name="key",value.name="target")
	
	return (df2)
}

#ids_cols:en:Column name string to be fixed (jp:固定する列名文字列) example: c("date_time", "deg_C", "relative_humidity")
#key: en:Column name string you want to lay down (jp:横にしたい列名文字列)
vertically_to_horizontally <- function(df, ids_cols, key="key")
{
	fomuler = ids_cols[1]
	for ( i in 2:length(ids_cols) )
	{
		fomuler = paste(fomuler, "+", ids_cols[i],sep="")
	}
	fomuler = paste(fomuler, "~ key",sep="")
	df3<-reshape2::dcast(df2, eval(parse(text =fomuler)) , value.var="target")
	
	return (df3)
}


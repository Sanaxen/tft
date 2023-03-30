install_libpath='C:/Users/bccat/Desktop/DDS2/DDS2/Temporal Fusion Transformers (TFT)/tft/script'
.libPaths(c(install_libpath,.libPaths()))

install.packages("bit64", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("R6", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("utf8", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("pkgconfig", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("foreach", repo="http://cran.r-project.org", lib=install_libpath) 

install.packages("ggplot2", repo="http://cran.r-project.org", dependencies=TRUE, lib=install_libpath) 
install.packages("plotly", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("tidyverse", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("gtools", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("labeling", repo="http://cran.r-project.org", lib=install_libpath) 

install.packages("remotes", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("rlang", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)


#remotes::install_github("mlverse/luz", lib=install_libpath)
install.packages("luz", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)

# CPU
Sys.setenv("CUDA_PATH"  = "")
Sys.setenv("CUDA_HOME"  = "")
install.packages("torch", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)

# CUDA
Sys.setenv(CUDA="11.3")
Sys.setenv("CUDA_PATH"  = "C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v11.3")
install.packages("torch", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
#remotes::install_github("mlverse/torch")

library(torch)
torch::torch_manual_seed(1)

remotes::install_github("mlverse/tft", lib=install_libpath)

install.packages("tidymodels", repos = "http://cran.us.r-project.org", lib=install_libpath)
install.packages("slider", repos = "http://cran.us.r-project.org", lib=install_libpath)
install.packages("gplots", repos = "http://cran.us.r-project.org", lib=install_libpath)




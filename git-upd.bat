if "%1" == "" goto USAGE
set MESSAGE="%1 %2 %3 %4 %5 %6 %7 %8 %9"
cd C:\Users\jimcl\source\repos\Able
git add -A .
git add -u .
git status
git commit -m %MESSAGE%     
@echo #################################################################
@echo #  (Cancel the GUI dialog. When it asks, enter your git creds)  #
@echo #################################################################
git push
goto DONE
:USAGE
@echo Usage: 
@echo   git-upd required-commit-message
@echo Note:
@echo   up to nine spaces are permitted in the message, due to Windows
:DONE
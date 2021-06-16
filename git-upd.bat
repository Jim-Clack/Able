if "%1" == "" goto USAGE
set MESSAGE="%1 %2 %3 %4 %5 %6 %7 %8 %9"
cd C:\Users\jimcl\source\repos\Able
git add -A .
git add -u .
git status
git commit -m %MESSAGE%     
@echo (Skip the GUI dialog, then if it asks for your username, enter your email instead)
git push
goto DONE
:USAGE
@echo Usage: 
@echo git-upd required commit message
:DONE
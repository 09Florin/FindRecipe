The project is a recipe manager where the administrators can add ingredients, recipes, categories of ingredients and other administrators.
The normal user can search for recipes either by their name if the user knows already what he wants to cook or by the main ingredient he has at home and he may not know what to do with.
Every recipe can by added to the user’s favorite lists which is displayed on his profile.
The only limitation of the application is that the user can search recipe by a single ingredient right now.

The user is restricted to only search recipes, view them, add to a favorite list and see his other profile details. The ones that can add recipes, edit them, add ingredients and so on are the administrators.

The application uses EntityFrameworkCore 8, therefore the database is built upon the model classes like Administrator, User, Category, Ingredient, Recipe, RecipeIngredient which links the ingredients to recipes because there is a many to many relationship, but also ViewModels for Login and Registration processes.

The framework supports scaffolding, so the project could generate automatically the controllers for each class which dictates the way every object behaves. These are the brain of the web application. Also the views are created automatically by scaffolding and represent the UI element of the project. 

The database contains a lot of ingredients and some recipes in order to test the application and runs locally on my personal computer.


The user interface is a simple one, with a pleasant tone of colors, but it lacks original and unique content. A great improvement would have been the positioning of the elements.

![image](https://github.com/user-attachments/assets/d3a0fc17-66c0-4300-891f-21243a5c5a44)



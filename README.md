# Wi Advice

### Story

An idea emerged while I was in unversity "Wouldn't it be great to have one place, in which you could find opinions for different products, before buying them..". I thought it was superb, couldn't find similar application and didn't had idea at the time for diploma project so I started working on it.

Less than year after I presented it as diploma thesis, after which decided that I can add couple more features, polish it and then try to publish it. An year more later I published it officially and had to start to market it. This is the point where my naive thoughts hit reality and I realized that just building product is far from enough for it's adoption.   

Soon after that I stopped working on the project and proceed to find job...

### About

This is my first (.NET) and longest project. It is an user driven system, in which users can express opinions and discuss products, with idea to help them gather enough information before making decisions.

It has:  
1. User side in which users can
  * Register with or without email (limited rights in this case)
  * Ability to add products/companies and update them while they have rights on them (an user who added company/product has rights to modify them, that doesn't include other products in the same company)
  * Give/or Transfer the rights for company/product to another user (the owner can give rights to more than one user)
  * Write comments, create topics, comment in topics for each product
  * Anonymous users can write comments (with captcha validation)
  * Subscribe for notifications: when new comments are written for product; on new products added for company; new topics for products
  * Send private messages to other users
  * Browse user profiles
  * Rate comments and products
  * Write suggestions for product/company information update > which can be resolved by their editors
  * Send reports (for bad behavior), mark comments as spam
  * Write suggestions for the site
  
2. Administrative side in which administrators can
  * Modify/Create new categories, in which products can be added and discussed  
  * Take actions based on user reports for spam, offensive comments
  * Send warnings to users based on reports or incorrect behavior. The users are automatically suspended when they receive fixed number of warnings.
  * Control advertisements, which appear next products comments
  * See various statistics and logs
  * Register other administrators with specific roles
  * Update the texts in specific places (blog, rules, FAQ...)
  * Give/Remove user roles
  * Suspend users
  * Make user change his username on next log in
  * Delete comments, topics, topic comments
  * Modify categories/companies/products: change names, move to other category/company, delete, remove/transfer their rights to other users...
  * IP Ban as measure against anonymous users
  
3. General funtionality
  * Search for products, categories, users, companies
  * Setting for choosing site width (personal for user)
  * Multilanguage, with separate databases for each language (meaning that the categories/companies/products..information is different than in the other languages), with common database for the users
  * Contact form for sending suggestions/questions/inquiries..
  * Captcha in registration, comments (for anonymous users)..
  * Max login tries, forgot password option, restrictions on number of comments/products/companies added for new/anonymous users

It is huge, in some aspects crude, with ugly interface on places and slow.

### Technologies

.NET 3.5, Web Forms, C#, Web Services, Entity Framework, log4net, URL Rewrite, AJAX, Javascript,  

### Poke/Edit

### Images

![alt text](https://github.com/raste/WiAdvice/blob/master/screenshots/Home.png "Home")

![alt text](https://github.com/raste/WiAdvice/blob/master/screenshots/Company.png "Company")

![alt text](https://github.com/raste/WiAdvice/blob/master/screenshots/Category.png "Category")

![alt text](https://github.com/raste/WiAdvice/blob/master/screenshots/Product.png "Product")

![alt text](https://github.com/raste/WiAdvice/blob/master/screenshots/Forum.png "Forum")

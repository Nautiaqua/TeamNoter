![Logo](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/th5xamgrr6se0x5ro4g6.png)


# TeamNoter
A task-management app designed for teams dependent on user-managed MySQL databases, portable and forever free.


## Installation & Setup

- Head into the releases page and download the latest release for TeamNoter.
- Extract the TeamNoter folder into your location of choice and launch the app.

Once the app is launched:
- If you're connecting to a pre-established TeamNoter-compatible database.
    - If your account hasn't already been created, request the owner or an admin to create you an account.
    - Log in using the database details provided by your organization and your account details.

If you're looking to set up a TeamNoter compatible database:
- Start your MySQL Server instance either locally or remotely and connect to it.
- Once connected, execute the SQL Files that came with the installation .zip folder.
- Execute INITIALIZE-DATABASE.sql, then OWNER-SETUP.sql (do not forget to change OWNER-SETUP.sql's placeholder details).
- With the owner account created, you may connect to the TeamNoter-compatible database using the app and log in.
- Create users when necessary, or begin using the app as is.

It is suggested to have different MySQL server users with varying permissions (depending on their role) when using TeamNoter. If you trust your team / are using the app by yourself, you may skip creating a non-admin MySQL server user.

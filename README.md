CREATE TABLE BooksPossess (
    OwnerId INT,
    BookId INT,
    
    FOREIGN KEY (OwnerId) REFERENCES [User](User_id),
    FOREIGN KEY (BookId) REFERENCES Book(book_id)
);
 if you are testing with the database , you have to add this

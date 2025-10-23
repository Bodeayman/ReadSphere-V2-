
/*
   [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddRating(BookDetailsViewModel model)
{
    if (!model.Rating.HasValue || string.IsNullOrWhiteSpace(model.Comment))
    {
        ModelState.AddModelError("", "Rating and comment are required.");
        return View("BookDetails", model); // return same view with errors
    }

    try
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string insertReview = @"
                    INSERT INTO review (Rating, Description, User_Id)
                    VALUES (@Rating, @Comment, @UserId);
                    SELECT SCOPE_IDENTITY();";

            int userId = 1; // replace with actual logged-in user id

            using (var cmd = new SqlCommand(insertReview, connection))
            {
                cmd.Parameters.AddWithValue("@Rating", model.Rating.Value);
                cmd.Parameters.AddWithValue("@Comment", model.Comment);
                cmd.Parameters.AddWithValue("@UserId", userId);

                int reviewId = Convert.ToInt32(cmd.ExecuteScalar());

                string insertBookReview = @"
                        INSERT INTO book_review (Book_Id, Review_Id)
                        VALUES (@BookId, @ReviewId)";

                using (var cmd2 = new SqlCommand(insertBookReview, connection))
                {
                    cmd2.Parameters.AddWithValue("@BookId", model.Id);
                    cmd2.Parameters.AddWithValue("@ReviewId", reviewId);
                    cmd2.ExecuteNonQuery();
                }
            }
        }

        // redirect to GET after successful post
        return RedirectToAction("Details", new { id = model.Id });
    }
    catch
    {
        ModelState.AddModelError("", "Error submitting rating.");
        return View("Details", model);
    }
}
    */